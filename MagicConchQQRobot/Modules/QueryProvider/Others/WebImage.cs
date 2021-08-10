using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Utils;
using MagicConchQQRobot.DataObjs;
using MagicConchQQRobot.DataObjs.DbClass;
using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.SecretProvider;
using MagicConchQQRobot.Modules.Utils;
using SauceNET;
using System;
using System.Collections.Generic;
using Convert = System.Convert;

namespace MagicConchQQRobot.Modules.QueryProvider.Others
{
    class WebImage
    {
        public static List<SauceNETClient> ClientList = new();

        public static void InitSauceNaoClients()
        {
            foreach (var item in SecretData.GetChildren("SauceNao:Clients"))
            {
                ClientList.Add(new SauceNETClient(item.Value));
            }
        }

        public static async System.Threading.Tasks.Task IdentityForImageUrlAsync(long groupId, long sendtoId, string url, bool isPixivOnly)
        {
            try
            {
                Random rnd = new();
                //Get the sauce
                var sauce = await ClientList[rnd.Next(0, ClientList.Count)].GetSauceAsync(url);

                int selectIndex = 0;
                if (isPixivOnly)
                {
                    for (int i = 0; i < sauce.Results.Count; i++)
                    {
                        if (sauce.Results[i].DatabaseName.Equals("Pixiv", StringComparison.OrdinalIgnoreCase))
                        {
                            selectIndex = i;
                            break;
                        }
                        else if (i == sauce.Results.Count - 1)
                        {
                            await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId,
                                new Message(new ElementText("抱歉，没有找到对应的仅Pixiv结果！您可以尝试去掉P站限定，也许可以从其他数据源找到对应的图片~（部分情况下通过其他图源的Source是可以找到对应的Pixiv图的）"), new ElementAt(sendtoId)));
                            return;
                        }
                    }
                }

                //Top result source url, if any.
                string source = sauce.Results[selectIndex].SourceURL;
                string thumbnailUrl = sauce.Results[selectIndex].ThumbnailURL;
                string similarity = sauce.Results[selectIndex].Similarity;
                string database = sauce.Results[selectIndex].DatabaseName;

                string outputText = $"\n图源：{database}";

                if (database.Equals("Pixiv"))
                {
                    outputText += $"\nP站ID：{sauce.Results[selectIndex].Properties.Find(c => c.Name.Equals("PixivId", StringComparison.Ordinal)).Value}" +
                        $"\n画师：{sauce.Results[selectIndex].Properties.Find(c => c.Name.Equals("MemberName", StringComparison.Ordinal)).Value}" +
                        $" 画师ID：{sauce.Results[selectIndex].Properties.Find(c => c.Name.Equals("MemberId", StringComparison.Ordinal)).Value}";
                }
                outputText += $"\n图片出处URL：{source}\n相似度：{similarity}%";
                if (Convert.ToSingle(similarity) < 50.0)
                {
                    outputText += $"\n【请注意，该图片相似度过低，很可能不是您要找的结果】";
                    outputText += $"\n【没有在网上公开发表过的图片（含微博图片）以及大多自制表情包是不在识别范围内的】";
                }
                User userItem = User.Find(User._.Uid == sendtoId);
                int identifyCount = 1;
                if (userItem != null)
                {
                    identifyCount = userItem.ImageIdCount;
                }
                outputText += $"\n您今日已查询次数{identifyCount}/5次，每日零点刷新次数，可申请增加次数。{(IdentityHelper.IsSuperNoobMember(sendtoId) ? "（您是尊贵的SuperNoob团队成员，无视次数限制）" : string.Empty)}";
                outputText += "\n※识图功能由Cyrus API Hub驱动，图片来自互联网，识别结果无法保障完全正确。";


                await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId,
                    new Message(new ElementImage(thumbnailUrl), new ElementText(outputText), new ElementAt(sendtoId)));
            }
            catch(Exception ex)
            {
                Console.WriteLine("识图模块发生故障："+ex.Message.ToString());
                Console.WriteLine(ex.StackTrace.ToString());
                await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId,
                     new Message(new ElementText(":(\n很抱歉，识图模块发生故障。\n错误原因已输出至系统后台，请联系我的主人处理该故障！"), new ElementAt(sendtoId)));
            }

        }

        public static void IncreaseDailyIdentifyCount(long groupId, long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null)
            {
                userItem.ImageIdCount++;
                userItem.Update();
            }
            else
            {
                Console.WriteLine($"正在更新群{groupId}的用户数据……");
                List<GroupUser> groupUsers = new();
                GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(groupId)).Result;
                Console.WriteLine($"群{groupId}目前有{groupMemberList.memberInfo.Length}个用户");
                string userSuperNoobId = string.Empty;
                foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                {
                    if (userInfo.user_id == userId)
                    {
                        userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                    }
                }
                if (string.IsNullOrEmpty(userSuperNoobId)) userSuperNoobId = GlobalObj.GroupRobotUserLists[groupId].Find(e => e.QQNumber == userId).Nickname;

                //如果数据库中没有这个用户的数据，就将该用户写入数据库
                userItem = IdentityHelper.UserRegister(userId, userSuperNoobId);
                userItem.ImageIdCount = 1;
                userItem.LastImageIdTime = TimestampHelper.ConvertToUnixOfTime(DateTime.Today).ToLong();
                userItem.Update();
            }
        }

        public static bool GetDailyIdentifyIsAvailable(long userId)
        {
            User userItem = User.Find(User._.Uid == userId);
            if (userItem != null)
            {
                if (userItem.LastImageIdTime == TimestampHelper.ConvertToUnixOfTime(DateTime.Today))
                {
                    if (userItem.ImageIdCount < 5)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    userItem.LastImageIdTime = TimestampHelper.ConvertToUnixOfTime(DateTime.Today).ToLong();
                    userItem.ImageIdCount = 0;
                    userItem.Update();
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
