using MagicConchQQRobot.DataObjs;
using MagicConchQQRobot.DataObjs.DbClass;
using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.AccountInfoProvider;
using MagicConchQQRobot.Modules.InteractiveProvider;
using MagicConchQQRobot.Modules.QueryProvider.Others;
using MagicConchQQRobot.Modules.QueryProvider.Pixiv;
using MagicConchQQRobot.Modules.Utils;
using MagicConchQQRobot.Presets;
using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Events.CQEvents;
using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;
using CyrusVance.CoolQRobot.Utils;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Convert = System.Convert;
using Directory = System.IO.Directory;

namespace MagicConchQQRobot
{
    class MessageProcess
    {
        public enum TextType
        {
            Unknown = 0,//未知功能
            RegisterRobotUser = 1,//用户是否需要注册正式用户功能
            Pa = 2,//爪巴功能
            EightHoursBanNeed = 3,//用户是否呼叫主动禁言
            QueryNeeded = 4,//用户是否需要查询功能
            ImageIdentify = 5,//用户是否需要Pixiv识图功能
            PixivImageSearch = 6,//用户是否需要Pixiv找图功能
            PixivAuthorSearch = 7,//用户是否需要Pixiv画师作品寻找功能
            YudouTransfer = 8,//用户是否需要鱼豆转移功能
            HunShuiRape = 9,//用户是否需要昏睡红茶雷普功能
            LearnImage = 10,//用户是否需要图片学习功能
            ImageNeeded = 11,//用户是否需要来张XX图功能
            SponsorNeeded = 12,//用户是否需要赞助功能
            MinecraftInfoNeeded = 13,//用户是否需要Minecraft相关信息获取功能
            LotteryNeeded = 14,//用户是否需要参与抽奖
            RapeMember = 15,//用户是否需要雷普群员功能
            NavySchool = 16,//用户是否需要海军学校语音功能
            MusicNeeded = 17,//用户是否需要音乐（语音形式）点播功能
            DianNiuZiNeeded = 18,//用户是否需要电牛子语音功能
        }
        public static async System.Threading.Tasks.Task ProcessGroupMessageAsync(GroupMessageEvent me)
        {
            User userObj = IdentityHelper.GetUserObj(me.sender.user_id);



            bool isRobotUser = IdentityHelper.IsRobotUser(userObj) || GroupData.GetSpecialServiceGroups().Contains(me.group_id);
            bool isAdmin = IdentityHelper.IsAdmin(me.sender.user_id);
            int userAccessLevel = IdentityHelper.GetUserAccessLevel(userObj);
            bool isUserBanned = IdentityHelper.IsUserBanned(userObj);

            FunnyImage.ImageType imageType = FunnyImage.ImageType.Unknown;

            var userInfoInGroup = GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == me.sender.user_id);

            bool isCommandModeOn = false;
            bool isBotCalled = false;
            TextType textType = TextType.Unknown;
            string queryText = ""; //用户请求查询功能

            Console.WriteLine("收到来自" + me.sender.user_id + "的消息:" + me.message.ToString());

            //Console.WriteLine(me.message.data[0].ToString());

            if (me.message.data[0].type.Equals("text"))
            {
                if (me.message.data[0].data["text"][0] == '/' && (isRobotUser || isAdmin))
                {
                    isCommandModeOn = true;
                }
                else if (me.message.data[0].data["text"][0] == '!' && isAdmin)
                {
                    //执行管理员指令
                    string commandContent = me.message.data[0].data["text"].Substring(1);
                    var paramList = commandContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    switch (paramList[0].ToLower())
                    {
                        case "allchat":
                            {
                                if (paramList.Length == 2)
                                {
                                    string switchText = paramList[1].ToLower();
                                    if (switchText.Equals("on"))
                                    {
                                        GlobalSet.IsAllChatServiceEnabled = true;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "互动系统的所有功能总开关已开启，现在将按正常情况响应所有请求！");

                                    }
                                    else if (switchText.Equals("off"))
                                    {
                                        GlobalSet.IsAllChatServiceEnabled = false;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "互动系统的所有功能总开关已关闭，在重新打开之前将不会再响应任何请求！");

                                    }
                                    else if (switchText.Equals("status"))
                                    {
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"当前互动系统的总开关状态：{(GlobalSet.IsAllChatServiceEnabled ? "开启" : "关闭")}");
                                    }
                                }
                                break;
                            }
                        case "debug":
                            {
                                string debugOnText = $"群{me.group_id} 调试模式已经开启，将开启向调试群中发送调试信息！";
                                string debugOffText = $"群{me.group_id} 调试模式已经关闭，将停止向调试群中发送调试信息！";
                                if (paramList.Length == 1)
                                {
                                    if (GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled)
                                    {
                                        GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled = false;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, debugOffText);
                                    }
                                    else
                                    {
                                        GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled = true;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, debugOnText);
                                    }
                                }
                                else if (paramList.Length == 2)
                                {
                                    string switchText = paramList[1].ToLower();
                                    if (switchText.Equals("on"))
                                    {
                                        GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled = true;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, debugOnText);
                                    }
                                    else if (switchText.Equals("off"))
                                    {
                                        GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled = false;
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, debugOffText);
                                    }
                                    else if (switchText.Equals("status"))
                                    {
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"群{me.group_id} 当前调试模式为{(GlobalObj.GroupInfoLists[me.group_id].IsDebugEnabled ? "开启" : "关闭")}状态");
                                    }
                                    else if (switchText.Equals("all-status"))
                                    {
                                        string sendText = "当前所有服务的群的调试状态如下：\n";
                                        foreach (var item in GlobalObj.GroupInfoLists)
                                        {
                                            sendText += $"群{item.Key}（{item.Value.GroupName}） 调试状态：{(item.Value.IsDebugEnabled ? "开启" : "关闭")}\n";
                                        }
                                        sendText += "如需开启或关闭相应群的调试功能，请在对应群内输入对应/debug [on/off]进行开启或关闭操作！";
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, sendText);

                                    }
                                }
                                break;
                            }
                        case "register":
                            {
                                bool isNotSuperNoobHQ = false;
                                if (paramList.Length == 2 && paramList[1].ToLower().Equals("not-supernoob-hq")) isNotSuperNoobHQ = true;
                                Console.WriteLine($"正在更新群{me.group_id}的用户数据……");
                                List<GroupUser> groupUsers = new();
                                GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                Console.WriteLine($"群{me.group_id}目前有{groupMemberList.memberInfo.Length}个用户");
                                foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                                {
                                    if (userInfo.user_id == GlobalObj.RobotQQ) continue;
                                    GroupUser user = new GroupUser()
                                    {
                                        QQNumber = userInfo.user_id,
                                        GroupNickname = userInfo.card,
                                        Nickname = userInfo.nickname,
                                    };
                                    if (isNotSuperNoobHQ)
                                    {
                                        user.SuperNoobID = userInfo.nickname;
                                    }
                                    else
                                    {
                                        string userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                                        if (!string.IsNullOrEmpty(userSuperNoobId))
                                        {
                                            user.SuperNoobID = userSuperNoobId;
                                        }
                                    }

                                    groupUsers.Add(user);
                                }

                                string wrongUserString = $"正在尝试向该群中写入{groupMemberList.memberInfo.Length - 1}个用户数据";
                                //群名片不合格的用户数量
                                int checkFailedUserExistCount = 0;
                                //已经注册的用户数量
                                int registeredUserCount = 0;
                                foreach (var item in groupUsers)
                                {
                                    User userItem = User.Find(User._.Uid == item.QQNumber);
                                    if (userItem == null)
                                    {
                                        if (!string.IsNullOrEmpty(item.SuperNoobID))
                                        {
                                            IdentityHelper.UserRegister(item.QQNumber, item.SuperNoobID);
                                        }
                                        else
                                        {
                                            checkFailedUserExistCount++;
                                            wrongUserString += $"\nID:{item.QQNumber} 群名片:{(string.IsNullOrEmpty(item.GroupNickname) ? "无" : item.GroupNickname)} 昵称:{item.Nickname}";
                                        }
                                    }
                                    else
                                    {
                                        if (item.SuperNoobID != null) userItem.Nickname = Base64Helper.Encode(item.SuperNoobID);
                                        userItem.Update();
                                        registeredUserCount++;
                                    }
                                }
                                if (checkFailedUserExistCount > 0) wrongUserString += $"\n以上{checkFailedUserExistCount}人由于未满足群名片标准，将不予注册！";
                                wrongUserString += $"\n已经注册在数据库的用户有{ registeredUserCount }人，实际写入新用户人数为{ groupMemberList.memberInfo.Length - checkFailedUserExistCount - registeredUserCount - 1}";
                                wrongUserString += $"\n以上写入的新用户默认等级为：无权限用户";
                                MessageHelper.SendGroupTextMessage(me.group_id, wrongUserString);
                                break;
                            }
                        case "adduser":
                            {
                                if (paramList.Length == 2)
                                {
                                    long targetUserId = paramList[1].ToLong();
                                    User user = User.Find(User._.Uid == targetUserId);
                                    if (user != null)
                                    {
                                        user.Level = 1;
                                        user.Update();
                                    }
                                    else
                                    {
                                        List<GroupUser> groupUsers = new List<GroupUser>();
                                        GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                        string userSuperNoobId = string.Empty;
                                        foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                                        {
                                            if (userInfo.user_id == targetUserId)
                                            {
                                                userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                                            }
                                        }
                                        if (string.IsNullOrEmpty(userSuperNoobId)) userSuperNoobId = "未知";

                                        //如果数据库中没有这个用户的数据，就将该用户写入数据库
                                        user = IdentityHelper.UserRegister(targetUserId, userSuperNoobId, 1);
                                    }
                                    MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户{targetUserId}已经成为正式用户！");

                                }
                                break;
                            }
                        case "r18":
                            {
                                if (paramList.Length == 3)
                                {
                                    if (paramList[2].Equals("on", StringComparison.Ordinal))
                                    {
                                        User user = User.Find(User._.Uid == paramList[1]);
                                        if (user != null)
                                        {
                                            user.Level = 2;
                                            user.Update();
                                            await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id,
                                                new Message(
                                                    new ElementAt(paramList[1].ToLong()),
                                                    new ElementText($"（{paramList[1]}）在经历了难以启齿的夜晚后，涩图请求功能已经开放了！\n她现在对应账户的权限等级为2")
                                                    )
                                                );
                                        }
                                    }
                                    else if (paramList[2].Equals("off", StringComparison.Ordinal))
                                    {
                                        User user = User.Find(User._.Uid == paramList[1]);
                                        if (user != null)
                                        {
                                            user.Level = 1;
                                            user.Update();
                                            await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id,
                                                new Message(
                                                    new ElementAt(paramList[1].ToLong()),
                                                    new ElementText($"（{paramList[1]}）的涩图请求功能已经关闭，现在对应账户的权限等级为1")
                                                    )
                                                );
                                        }
                                    }
                                }
                                break;
                            }
                        case "checkimage":
                            {
                                FunnyImage.CheckAllImageDirectory(me.group_id);
                                break;
                            }
                        case "cleanimage":
                            {
                                if (paramList.Length == 2)
                                {
                                    FunnyImage.ClearAllImages((FunnyImage.ImageType)paramList[1].ToInt(), me.group_id);
                                }

                                break;
                            }
                        case "give-bait":
                            {
                                if (paramList.Length == 3)
                                {
                                    Member member = Member.Find(Member._.qq == paramList[1]);
                                    if (member != null)
                                    {
                                        MemberScore memberScore = MemberScore.Find(MemberScore._.id == member.id);
                                        if (memberScore != null)
                                        {
                                            memberScore.Bait += Convert.ToSingle(paramList[2]);
                                            memberScore.Update();
                                        }
                                        else
                                        {
                                            memberScore = new MemberScore
                                            {
                                                id = member.id,
                                                Bait = Convert.ToSingle(paramList[2])
                                            };
                                            memberScore.Insert();
                                        }
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"已为{paramList[1]}{(Convert.ToSingle(paramList[2]) >= 0 ? "给予" : "扣除")}{Math.Abs(Convert.ToSingle(paramList[2]))}鱼饵，该账户现在所持鱼饵数量为{memberScore.Bait}.");
                                    }
                                    else
                                    {
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"{paramList[1]}并不是登记在库的SuperNoob成员，请手动添加！");
                                    }
                                }
                                break;
                            }
                        case "collect-sunb":
                            {
                                GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                Console.WriteLine($"群{me.group_id}目前有{groupMemberList.memberInfo.Length}个用户");

                                int successCount = 0;
                                int failedCount = 0;
                                int existedCount = 0;
                                string failedString = "";

                                List<Member> members = new List<Member>();
                                foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                                {
                                    if (userInfo.user_id == GlobalObj.RobotQQ) continue;
                                    Member member = new()
                                    {
                                        qq = userInfo.user_id,
                                        JoinTime = userInfo.join_time
                                    };
                                    string userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                                    if (!string.IsNullOrEmpty(userSuperNoobId))
                                    {
                                        member.Code = userSuperNoobId;
                                        Member existedMember = Member.Find(Member._.qq == userInfo.user_id);

                                        if (existedMember == null)
                                        {
                                            members.Add(member);
                                            successCount++;
                                        }
                                        else
                                        {
                                            existedCount++;
                                        }
                                    }
                                    else
                                    {
                                        failedString += $"ID:{userInfo.user_id} 群名片:{(string.IsNullOrEmpty(userInfo.card) ? "无" : userInfo.card)} 昵称:{userInfo.nickname}\n";
                                        failedCount++;
                                    }

                                }

                                members.Sort((x, y) => x.JoinTime.CompareTo(y.JoinTime));
                                for (int i = 0; i < members.Count; i++)
                                {
                                    Member item = members[i];
                                    item.id = i + 1;
                                    item.Insert();
                                }
                                MessageHelper.SendGroupTextMessage(me.group_id, failedString + $"已采集{successCount}/{groupMemberList.memberInfo.Length}位队员的数据，已经写入过数据库的队员有{existedCount}位{(failedCount != 0 ? $"，其中有以上{failedCount}位队员由于群名片格式不正确将无法获得队员身份！" : "")}");

                                break;
                            }
                        case "lottery":
                            {
                                if (paramList.Length >= 2)
                                {
                                    switch (paramList[1])
                                    {
                                        case "start":
                                            {
                                                if (!GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                                {
                                                    if (paramList.Length == 3)
                                                    {
                                                        GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled = true;
                                                        GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName = paramList[2];
                                                        MessageHelper.SendGroupTextMessage(me.group_id, $"抽奖活动已经开始！本期抽奖主题为【{paramList[2]}】，快发送“神奇海螺，参与抽奖”参与这期抽奖吧！");
                                                    }
                                                }
                                                else
                                                {
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"目前【{GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName}】抽奖活动正在进行中！");
                                                }
                                                break;
                                            }
                                        case "list":
                                            {
                                                if (GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                                {
                                                    //更新群员列表
                                                    GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                                    GlobalObj.GroupMemberLists[me.group_id] = groupMemberList;

                                                    string displayedUsers = string.Empty;
                                                    foreach (var item in GlobalObj.GroupInfoLists[me.group_id].LotteryUserList)
                                                    {
                                                        var memberInfo = GlobalObj.GroupMemberLists[me.group_id].memberInfo.Find(e => e.user_id == item);
                                                        string username = memberInfo == null ? "新用户" : string.IsNullOrEmpty(memberInfo.card) ? memberInfo.nickname : memberInfo.card;
                                                        displayedUsers += $"\n{username}({item})";
                                                    }
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"参与【{GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName}】的用户名单如下：" + displayedUsers);
                                                }
                                                else
                                                {
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"目前本群并没有已经开始的抽奖活动哦~");
                                                }
                                                break;
                                            }
                                        case "result":
                                            {
                                                if (GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                                {
                                                    //更新群员列表
                                                    GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                                    GlobalObj.GroupMemberLists[me.group_id] = groupMemberList;

                                                    Random rnd = new();
                                                    long prizeWinner = GlobalObj.GroupInfoLists[me.group_id].LotteryUserList[rnd.Next(0, GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Count)];

                                                    if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                                                    string tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());
                                                    HttpHelper.DownloadFile(Get.UserAvatarUrl(Convert.ToUInt64(prizeWinner)), tmpFileName);

                                                    FileStream fs = new(tmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                                                    BinaryReader br = new(fs);
                                                    byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                                                    fs.Close();

                                                    MemoryStream ms = new();
                                                    ImageProcessHelper.Resize(Image.Load(imgBytesIn), 64, 64).SaveAsJpeg(ms);

                                                    File.Delete(tmpFileName);

                                                    var member = GlobalObj.GroupMemberLists[me.group_id].memberInfo.Find(e => e.user_id == prizeWinner);
                                                    GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id,
                                                        new Message(new ElementText($"本期【{GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName}】抽奖活动的获得者为："), new ElementImage(ms.ToArray()), new ElementText(member == null ? "新用户" : string.IsNullOrEmpty(member.card) ? member.nickname : member.card),
                                                        new ElementText("\n请获奖者"), new ElementAt(prizeWinner), new ElementText("发表获奖感言！"))).Wait();

                                                    GlobalObj.GroupInfoLists[me.group_id].LotteryUserList = new List<long>();
                                                    GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName = string.Empty;
                                                    GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled = false;
                                                }
                                                else
                                                {
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"目前本群并没有已经开始的抽奖活动哦~");
                                                }
                                                break;
                                            }
                                        case "force-add":
                                            {
                                                if (GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                                {
                                                    if (paramList.Length == 3)
                                                    {
                                                        if (paramList[2].Contains(','))
                                                        {
                                                            var userAccounts = paramList[2].Split(',', StringSplitOptions.RemoveEmptyEntries);
                                                            foreach (var item in userAccounts)
                                                            {
                                                                GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Add(item.ToLong());
                                                            }
                                                            MessageHelper.SendGroupTextMessage(me.group_id, $"已经强制添加{userAccounts.Length}名活动参与用户！");
                                                        }
                                                        else
                                                        {
                                                            GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Add(Convert.ToInt64(paramList[2]));
                                                            MessageHelper.SendGroupTextMessage(me.group_id, $"已经强制添加1名活动参与用户！");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"目前本群并没有已经开始的抽奖活动哦~");
                                                }
                                                break;
                                            }
                                        case "stop":
                                            {
                                                if (GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                                {
                                                    GlobalObj.GroupInfoLists[me.group_id].LotteryUserList = new List<long>();
                                                    GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName = string.Empty;
                                                    GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled = false;
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"本期抽奖已经强制结束！");
                                                }
                                                else
                                                {
                                                    MessageHelper.SendGroupTextMessage(me.group_id, $"目前本群并没有已经开始的抽奖活动哦~");
                                                }
                                                break;
                                            }
                                    }
                                }
                                break;
                            }
                        case "blacklist":
                            {
                                if (paramList.Length == 2)
                                {
                                    long targetUserId = paramList[1].ToLong();
                                    User targetUserObj = User.Find(User._.Uid == targetUserId);
                                    if (targetUserObj != null)
                                    {
                                        targetUserObj.Banned = 1;
                                        targetUserObj.Update();
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户{GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == targetUserId).Nickname}({targetUserId})已被定位并且被永久禁止与本系统互动，如需申诉请联系相关管理者。");
                                    }
                                    else
                                    {
                                        User registeredUser = IdentityHelper.UserRegister(me.sender.user_id, "被封禁的用户");
                                        registeredUser.Banned = 1;
                                        registeredUser.Update();
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户{GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == targetUserId).Nickname}({targetUserId})已被添加到数据库并且被永久禁止与本系统互动，如需申诉请联系相关管理者。");
                                    }
                                    Console.WriteLine($"用户{targetUserId}已被封禁");
                                }
                                else if (paramList.Length == 3)
                                {
                                    long targetUserId = paramList[1].ToLong();
                                    User targetUserObj = User.Find(User._.Uid == targetUserId);
                                    if (paramList[2].ToLower().Equals("unban", StringComparison.InvariantCulture))
                                    {
                                        if (targetUserObj != null)
                                        {
                                            targetUserObj.Banned = 0;
                                            targetUserObj.Update();
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户{targetUserId}已被解除封禁，现已可以与本系统互动。");
                                            Console.WriteLine($"用户{targetUserId}已被解除封禁");
                                        }
                                        else
                                        {
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户{GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == targetUserId).Nickname}({targetUserId})不存在于数据库中！");
                                        }

                                    }
                                }
                                break;
                            }
                        case "unregistersunb":
                            {
                                if (paramList.Length == 2)
                                {
                                    long targetUserId = paramList[1].ToLong();
                                    Member targetUserObj = Member.Find(Member._.qq == targetUserId);
                                    if (targetUserObj != null)
                                    {
                                        targetUserObj.Status = 2;
                                        targetUserObj.Update();
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"原SuperNoob队员 代号{targetUserObj.Code}({targetUserId})已被标记为退队成员并被取消所有系统访问资格，相关访问权限已禁用，未领取的福利、实物奖品等已作废。\n每位队员都是各位的伙伴，好好珍惜彼此吧~");
                                    }
                                    else
                                    {
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"用户不存在，请检查相关输入！");
                                    }
                                    Console.WriteLine($"用户{targetUserId}已被封禁");
                                }
                                break;
                            }
                    }
                }
            }

            if (GlobalSet.IsAllChatServiceEnabled && !isUserBanned)
            {
                TimeDelay.RandomPause();
                if (!isCommandModeOn)
                {
                    foreach (var item in me.message.data)
                    {
                        switch (item.type)
                        {
                            case "text":
                                string recieveData = item.data["text"];
                                if (recieveData.Contains("神奇海螺"))
                                {
                                    isBotCalled = true;
                                }

                                if (recieveData.ToUpper().Contains("申请") && recieveData.ToUpper().Contains("正式"))
                                {
                                    textType = TextType.RegisterRobotUser;
                                }
                                else if (recieveData.Contains("谁爬") || recieveData.Contains("谁爪巴") || recieveData.Contains("需要爬") || (recieveData.Contains("最") && (recieveData.Contains("爬") || recieveData.Contains("爪巴"))))
                                {
                                    textType = TextType.Pa;
                                }
                                else if (recieveData.Contains("精致睡眠") || recieveData.Contains("群主") && recieveData.Contains("傻逼") || ((recieveData.Contains("给我") || recieveData.Contains("让我")) && (recieveData.Contains("打钱") || recieveData.Contains("赞助") || recieveData.Contains("草") || recieveData.Contains("日"))))
                                {
                                    textType = TextType.EightHoursBanNeed;
                                }
                                else if (recieveData.Contains("昏睡红茶"))
                                {
                                    textType = TextType.HunShuiRape;
                                }
                                else if (recieveData.Contains("学习"))
                                {
                                    textType = TextType.LearnImage;
                                    if (recieveData.Contains("粪图") || recieveData.Contains("恶臭") || recieveData.Contains("先辈")) imageType = FunnyImage.ImageType.Xianbei;
                                    else if (recieveData.Contains("龙图") || recieveData.Contains("龙玉涛") || recieveData.Contains("杀妈") || recieveData.Contains("杀马")) imageType = FunnyImage.ImageType.Lyt;
                                    else imageType = FunnyImage.ImageType.Unknown;
                                }
                                else if (recieveData.Contains("查询"))
                                {
                                    textType = TextType.QueryNeeded;
                                    queryText = recieveData;
                                }
                                else if (recieveData.Contains("识图"))
                                {
                                    textType = TextType.ImageIdentify;
                                }
                                else if (recieveData.ToUpper().Contains("P站找图") || recieveData.ToUpper().Contains("PIXIV"))
                                {
                                    textType = TextType.PixivImageSearch;
                                }
                                else if (recieveData.ToUpper().Contains("画师"))
                                {
                                    textType = TextType.PixivAuthorSearch;
                                }
                                else if (recieveData.ToUpper().Contains("转移鱼豆"))
                                {
                                    textType = TextType.YudouTransfer;
                                }
                                else if (recieveData.ToUpper().Contains("来张"))
                                {
                                    textType = TextType.ImageNeeded;
                                }
                                else if (recieveData.ToUpper().Contains("来首"))
                                {
                                    textType = TextType.MusicNeeded;
                                }
                                else if (recieveData.ToUpper().Contains("Minecraft") || recieveData.ToUpper().Contains("MC") || recieveData.ToUpper().Contains("我的世界"))
                                {
                                    textType = TextType.SponsorNeeded;
                                }
                                else if (recieveData.ToUpper().Contains("赞助") || recieveData.ToUpper().Contains("打钱"))
                                {
                                    textType = TextType.SponsorNeeded;
                                }
                                else if (recieveData.ToUpper().Contains("参与抽奖"))
                                {
                                    textType = TextType.LotteryNeeded;
                                }
                                else if (recieveData.ToUpper().Contains("雷普"))
                                {
                                    textType = TextType.RapeMember;
                                }
                                else if (recieveData.ToUpper().Contains("海军") || recieveData.ToUpper().Contains("听不见") || recieveData.ToUpper().Contains("天山") || recieveData.ToUpper().Contains("泰罗") || recieveData.ToUpper().Contains("精神") || recieveData.ToUpper().Contains("天尊") || recieveData.ToUpper().Contains("东京") || recieveData.ToUpper().Contains("严守") || recieveData.ToUpper().Contains("军舰"))
                                {
                                    textType = TextType.NavySchool;
                                }
                                else if (recieveData.ToUpper().Contains("牛子"))
                                {
                                    textType = TextType.DianNiuZiNeeded;
                                }
                                break;
                            case "at":
                                long atUserQQ = Convert.ToInt64(item.data["qq"]);
                                if (atUserQQ == GlobalObj.RobotQQ)
                                {
                                    isBotCalled = true;
                                }
                                break;
                        }
                    }
                }

                //如果触发了前面的识图请求
                if (me.message.data[0].type.Equals("image"))
                {
                    if (userInfoInGroup.ImageIdentityNeededMode != 0)
                    {
                        int userIndex = GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id);
                        if (WebImage.GetDailyIdentifyIsAvailable(me.sender.user_id) || IdentityHelper.IsSuperNoobMember(me.sender.user_id))
                        {
                            bool isPixivOnlyNeeded = GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == me.sender.user_id).ImageIdentityNeededMode == 2;
                            foreach (var item in me.message.data)
                            {
                                if (item.type.Equals("image"))
                                {
                                    WebImage.IncreaseDailyIdentifyCount(me.group_id, me.sender.user_id);
                                    string image = item.data["file"];
                                    await WebImage.IdentityForImageUrlAsync(me.group_id, me.sender.user_id, image, isPixivOnlyNeeded);
                                    if (!isAdmin) break;
                                }
                            }
                        }
                        else
                        {
                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "您今日的查询次数已用完，请等到明天0点后再查询吧！");
                        }
                        GlobalObj.GroupRobotUserLists[me.group_id][userIndex].ImageIdentityNeededMode = 0;
                    }
                    else if (userInfoInGroup.ImageLearnNeededMode != 0)
                    {
                        foreach (var item in me.message.data)
                        {
                            if (item.type.Equals("image"))
                            {

                                string image = item.data["file"];

                                FunnyImage.LearnImages((FunnyImage.ImageType)userInfoInGroup.ImageLearnNeededMode, me.group_id, me.sender.user_id, image);
                                if (!isAdmin) break;
                            }
                        }
                        int userIndex = GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id);
                        GlobalObj.GroupRobotUserLists[me.group_id][userIndex].ImageLearnNeededMode = 0;
                    }
                }


                if (isBotCalled || isCommandModeOn || textType == TextType.EightHoursBanNeed)
                {
                    if (textType == TextType.RegisterRobotUser)
                    {
                        Member member = Member.Find(Member._.qq == me.sender.user_id);
                        if (member != null)
                        {
                            User robotUser = User.Find(User._.Uid == me.sender.user_id);
                            if (robotUser != null && robotUser.Level > 0)
                            {
                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "您已经是海螺的正式用户了，不需要再次申请~");
                                return;
                            }
                            MemberScore memberScore = MemberScore.Find(MemberScore._.id == member.id);
                            if (memberScore != null && memberScore.Bait >= 100)
                            {
                                if (robotUser == null)
                                {
                                    List<GroupUser> groupUsers = new();
                                    GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                                    string userSuperNoobId = string.Empty;
                                    foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                                    {
                                        if (userInfo.user_id == me.sender.user_id)
                                        {
                                            userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                                        }
                                    }
                                    if (string.IsNullOrEmpty(userSuperNoobId)) userSuperNoobId = "未知";
                                    robotUser = IdentityHelper.UserRegister(me.sender.user_id, userSuperNoobId);
                                }
                                else
                                {
                                    robotUser.Level = 1;
                                    robotUser.Update();
                                }
                                memberScore.Bait -= 100;
                                memberScore.Update();
                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"注册成功，恭喜您花费100鱼饵成为了海螺正式用户~！您现在的鱼饵余额为{memberScore.Bait}.");
                                return;
                            }
                        }
                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "您的鱼饵余额不足，注册失败~"); ;
                    }
                    else if (isRobotUser)
                    {
                        if (isCommandModeOn)
                        {
                            string commandContent = me.message.data[0].data["text"].Substring(1);
                            var paramList = commandContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            switch (paramList[0].ToLower())
                            {
                                case "rank":
                                    if (paramList.Length == 2 || paramList.Length == 3)
                                    {
                                        if (paramList[1].ToLower().Equals("help") || paramList[1].Equals("?"))
                                        {
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(false));
                                            break;
                                        }

                                        string[] serverList = { "日", "台", "国" };
                                        for (int i = 0; i < serverList.Length; i++)
                                        {
                                            if (paramList[1].Contains(serverList[i]))
                                            {
                                                bool isRefreshNeeded = false;
                                                if (paramList.Length == 3)
                                                {
                                                    if (paramList[2].Equals("refresh") && isAdmin)
                                                    {
                                                        isRefreshNeeded = true;
                                                    }
                                                    else
                                                    {
                                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(true));
                                                        break;
                                                    }
                                                }
                                                if (!Command.QueryPCRRankAsync(me, i + 1, isRefreshNeeded).Result)
                                                {
                                                    MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(true));
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                    MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(true));

                                    break;
                                case "hqcd":
                                    {
                                        bool isRefreshNeeded = false;
                                        if (paramList.Length == 2)
                                        {
                                            if (paramList[1].Equals("refresh") && isAdmin)
                                            {
                                                isRefreshNeeded = true;
                                            }
                                            else
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.Wrong_CommandUse);
                                                break;
                                            }
                                        }
                                        await Command.QueryHQCDAsync(me, isRefreshNeeded);
                                    }
                                    break;
                                case "savage":
                                    if (paramList.Length == 3)
                                    {
                                        await Command.QueryFFXIVAsync(me, paramList[1], paramList[2]);
                                        break;
                                    }
                                    else if (paramList.Length == 2)
                                    {
                                        if (paramList[1].ToLower().Equals("help") || paramList[1].Equals("?"))
                                        {
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.FFXIV_Savage(false));
                                        }
                                        break;
                                    }
                                    MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.FFXIV_Savage(true));
                                    break;
                                case "leipu":
                                    {
                                        if ((isAdmin || userAccessLevel >= 100) && paramList.Length == 2)
                                        {
                                            Regex rx = new Regex("^[0-9]*$");
                                            if (rx.IsMatch(paramList[1]) && GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == me.sender.user_id).RapeUserList.Count >= paramList[1].ToInt())
                                            {
                                                int rapeIndex = paramList[1].ToInt();
                                                if (rapeIndex >= 0)
                                                {
                                                    FunnyImage.RapeGroupMember(me.group_id, string.Empty, 0, GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == me.sender.user_id).RapeLevel, true, GlobalObj.GroupRobotUserLists[me.group_id].Find(e => e.QQNumber == me.sender.user_id).RapeUserList[rapeIndex]);
                                                    GlobalObj.GroupRobotUserLists[me.group_id][GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id)].RapeUserList = new List<GroupUser>();
                                                    GlobalObj.GroupRobotUserLists[me.group_id][GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id)].RapeLevel = 0;
                                                }
                                            }
                                        }
                                        break;

                                    }
                            }
                        }
                        else
                        {
                            switch (textType)
                            {
                                case TextType.Pa:
                                    {
                                        GroupUser groupUser;
                                        Random rnds = new Random();
                                        while (true)
                                        {
                                            groupUser = GlobalObj.GroupRobotUserLists[me.group_id][rnds.Next(0, GlobalObj.GroupRobotUserLists[me.group_id].Count)];
                                            if (!IdentityHelper.IsAdmin(groupUser.QQNumber) && groupUser.QQNumber != GlobalObj.RobotQQ) break;
                                        }
                                        string sendText = string.Format(PresetsText.PaText[rnds.Next(PresetsText.PaText.Length)], string.IsNullOrEmpty(groupUser.SuperNoobID) ? groupUser.Nickname : groupUser.SuperNoobID);

                                        if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                                        string tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());
                                        HttpHelper.DownloadFile(Get.UserAvatarUrl(Convert.ToUInt64(groupUser.QQNumber)), tmpFileName);

                                        FileStream fs = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                                        BinaryReader br = new BinaryReader(fs);
                                        byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                                        fs.Close();

                                        MemoryStream ms = new MemoryStream();
                                        ImageProcessHelper.Resize(Image.Load(imgBytesIn), 64, 64).SaveAsJpeg(ms);

                                        await GlobalObj.WSRobotClient.SendMessageAsync(
                                            MessageType.group_,
                                            me.group_id,
                                                    new Message(
                                                new ElementText(sendText), new ElementImage(ms.ToArray(), false)
                                            //new ElementText("\n艾特当事人功能已经被开发者关闭，如需艾特出来请自行操作，敬请谅解~")
                                            //new ElementAt(groupUser.QQNumber)
                                            ));
                                        ms.Close();
                                        File.Delete(tmpFileName);
                                        break;
                                    }
                                case TextType.EightHoursBanNeed:
                                    {
                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "如您所愿~");
                                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 28800)).Wait();
                                        break;
                                    }
                                case TextType.QueryNeeded:
                                    {
                                        if (queryText.Contains("国服零式"))
                                        {
                                            if (queryText.Contains(' '))
                                            {
                                                int textIndex = queryText.LastIndexOf("国服零式") + 5;
                                                if (queryText.Length > textIndex)
                                                {
                                                    string queryString = queryText.Substring(textIndex);
                                                    if (queryString.Contains(' '))
                                                    {
                                                        string[] textList = queryString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                                        if (textList.Length == 2)
                                                        {
                                                            await Command.QueryFFXIVAsync(me, textList[0], textList[1]);
                                                            return;
                                                        }

                                                    }
                                                }
                                            }
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.FFXIV_Savage(true));
                                        }
                                        else if (queryText.ToLower().Contains("rank"))
                                        {
                                            int queryId = 0;
                                            string[] serverList = { "日", "台", "国" };
                                            for (int i = 0; i < serverList.Length; i++)
                                            {
                                                if (queryText.Contains(serverList[i]))
                                                {
                                                    queryId = i + 1;
                                                    break;
                                                }
                                            }
                                            if (queryId != 0)
                                            {
                                                if (!Command.QueryPCRRankAsync(me, queryId).Result)
                                                {
                                                    MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(true));
                                                }
                                            }
                                            else
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, HelpText.PCR_Rank(true));
                                            }
                                        }
                                        else if (queryText.Contains("黄骑") && queryText.Contains("充电"))
                                        {
                                            await Command.QueryHQCDAsync(me);
                                        }
                                        else if (queryText.Contains("特朗普") || queryText.Contains("川普") || queryText.Contains("拜登"))
                                        {
                                            UovzVPN.MachineStateReturn machineStateReturn = UovzVPN.GetMachineNetworkState();
                                            if (machineStateReturn != null)
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"当前身份可用，[拜登 - HK节点]可用上行流量当前使用流量状况:{machineStateReturn.UsedData}/{machineStateReturn.TotalData}，剩余流量：{machineStateReturn.RemainData}");
                                            }
                                            else
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"获取特朗普消息发生错误，请及时联系开发者！");
                                            }
                                        }
                                        else if (queryText.Contains("鱼饵余额"))
                                        {
                                            long queryQQ = me.sender.user_id;

                                            if (isAdmin && queryText.Contains(' '))
                                            {
                                                int textIndex = queryText.LastIndexOf("鱼饵余额") + 5;
                                                if (queryText.Length > textIndex)
                                                {
                                                    string queryString = queryText[textIndex..];
                                                    string[] textList = queryString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                                    if (textList.Length == 1)
                                                    {
                                                        queryQQ = textList[0].ToLong();
                                                    }
                                                }
                                            }

                                            Member member = Member.Find(Member._.qq == queryQQ);
                                            if (member != null)
                                            {
                                                MemberScore memberScore = MemberScore.FindByid(member.id);
                                                float userBaitNum = 0.00f;
                                                if (memberScore != null)
                                                {
                                                    userBaitNum = memberScore.Bait;
                                                }
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"{queryQQ}现在拥有的鱼饵数量为：{userBaitNum}");
                                            }
                                            else
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"{queryQQ}不存在于数据库中，无法查询到相关数据！");
                                            }
                                        }
                                        break;
                                    }
                                case TextType.ImageIdentify:
                                    {
                                        if (WebImage.GetDailyIdentifyIsAvailable(me.sender.user_id) || IdentityHelper.IsSuperNoobMember(me.sender.user_id))
                                        {
                                            bool isImageFoundInMessage = false;
                                            bool isPixivOnly = false;
                                            foreach (var msgItem in me.message.data)
                                            {
                                                if (msgItem.type.Equals("text", StringComparison.Ordinal))
                                                {
                                                    if (msgItem.data["text"].ToLower().Contains("p站") || msgItem.data["text"].ToLower().Contains("pixiv"))
                                                    {
                                                        isPixivOnly = true;
                                                    }
                                                }
                                                if (msgItem.type.Equals("image", StringComparison.Ordinal))
                                                {
                                                    WebImage.IncreaseDailyIdentifyCount(me.group_id, me.sender.user_id);
                                                    string image = msgItem.data["file"];

                                                    await WebImage.IdentityForImageUrlAsync(me.group_id, me.sender.user_id, image, isPixivOnly);

                                                    isImageFoundInMessage = true;

                                                    if (!isAdmin) break;
                                                }
                                            }
                                            if (!isImageFoundInMessage)
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"现在发送您要识别的图片即可开始识别！{(isPixivOnly ? "（仅返回Pixiv结果）" : null)}");
                                                int userIndex = GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id);
                                                GlobalObj.GroupRobotUserLists[me.group_id][userIndex].ImageIdentityNeededMode = isPixivOnly ? 2 : 1;
                                            }
                                        }
                                        else
                                        {
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "您今日的查询次数已用完，请等到明天0点后再查询吧！");
                                        }
                                        break;
                                    }
                                case TextType.PixivImageSearch:
                                    {
                                        foreach (var item in me.message.data)
                                        {
                                            if (item.type.Equals("text", StringComparison.Ordinal) && item.data["text"].Contains(' ') && item.data["text"].ToUpper().Contains("ID"))
                                            {
                                                string paramText = item.data["text"].Substring(item.data["text"].ToUpper().IndexOf("ID") + 2);
                                                var paramList = paramText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                                int selectedIndex = 0;
                                                string queryImageId = string.Empty;
                                                if (paramList[0].Contains('-'))
                                                {
                                                    var analyseParams = paramList[0].Split('-', StringSplitOptions.RemoveEmptyEntries);
                                                    queryImageId = analyseParams[0];
                                                    selectedIndex = analyseParams[1].ToInt() - 1;
                                                }
                                                else
                                                {
                                                    queryImageId = paramList[0];
                                                }

                                                if (paramList.Length >= 1)
                                                {
                                                    await ImjadApi.GetImageDetailAsync(me.group_id, me.sender.user_id, queryImageId, selectedIndex, paramText.Contains("原图"));
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case TextType.PixivAuthorSearch:
                                    {
                                        if (userAccessLevel >= 2 || isAdmin)
                                        {
                                            foreach (var item in me.message.data)
                                            {
                                                if (item.type.Equals("text", StringComparison.Ordinal) && item.data["text"].ToUpper().Contains("ID"))
                                                {
                                                    string paramText = item.data["text"].Substring(item.data["text"].ToUpper().IndexOf("ID") + 2);
                                                    var paramList = paramText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                                    if (paramList.Length >= 1)
                                                    {
                                                        await ImjadApi.GetUserIllustListAsync(me.group_id, me.sender.user_id, paramList[0], paramList.Length == 2 ? paramList[1].ToInt() : 1);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case TextType.HunShuiRape:
                                    {
                                        if (isAdmin)
                                        {
                                            string messageContent = me.message.data[0].data["text"];
                                            if (me.message.data[0].type.Equals("text", StringComparison.Ordinal))
                                            {
                                                int geiIndex = messageContent.IndexOf("给");
                                                int laiIndex = messageContent.IndexOf("来");
                                                int yibeiIndex = messageContent.IndexOf("一杯");

                                                string memberName = "";
                                                if (laiIndex != -1)
                                                {
                                                    memberName = messageContent[(geiIndex + 1)..laiIndex];
                                                }
                                                else if (yibeiIndex != -1)
                                                {
                                                    memberName = messageContent[(geiIndex + 1)..yibeiIndex];
                                                }

                                                int rapeLevel = 0;
                                                if (messageContent.Contains("加冰"))
                                                {
                                                    rapeLevel += 1;

                                                }
                                                if (messageContent.Contains("超级"))
                                                {
                                                    rapeLevel += 1;
                                                }
                                                FunnyImage.RapeSuNBMember(me.group_id, memberName, rapeLevel);
                                            }
                                        }
                                        break;
                                    }
                                case TextType.LearnImage:
                                    {
                                        if (userAccessLevel >= 2 || isAdmin)
                                        {
                                            if (imageType == FunnyImage.ImageType.Unknown)
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"请指定您需要学习的图片类型！（例：龙图、粪图）");
                                                return;
                                            }
                                            bool isIncludeImage = false;
                                            foreach (var item in me.message.data)
                                            {
                                                if (item.type.Equals("image", StringComparison.Ordinal))
                                                {
                                                    FunnyImage.LearnImages(imageType, me.group_id, me.sender.user_id, item.data["file"]);
                                                    isIncludeImage = true;
                                                }
                                            }
                                            if (!isIncludeImage)
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"现在发送您要让我学习的图片即可让我记住这张图！");
                                                int userIndex = GlobalObj.GroupRobotUserLists[me.group_id].FindIndex(e => e.QQNumber == me.sender.user_id);
                                                GlobalObj.GroupRobotUserLists[me.group_id][userIndex].ImageLearnNeededMode = (int)imageType;
                                            }
                                        }
                                        break;
                                    }
                                case TextType.ImageNeeded:
                                    {
                                        foreach (var item in me.message.data)
                                        {
                                            if (item.type.Equals("text", StringComparison.Ordinal))
                                            {
                                                string messageContent = item.data["text"];
                                                if (messageContent.Contains("先辈") || messageContent.Contains("粪图") || messageContent.Contains("恶臭"))
                                                {
                                                    List<Element> elements = new List<Element>();
                                                    byte[] imgBytes = FunnyImage.TryFindPhoto(FunnyImage.ImageType.Xianbei);
                                                    if (imgBytes != null)
                                                    {
                                                        elements.Add(new ElementImage(imgBytes));
                                                        elements.Add(new ElementText("\n精神百倍！今天也是元气满满地一天呢！！适合和先辈逛街哦~"));
                                                        elements.Add(new ElementAt(me.sender.user_id));
                                                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(elements.ToArray()));
                                                    }
                                                    else
                                                    {
                                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "粪图图库里没有数据哦~快让我变得更臭吧！！哼，哼，我已经迫不及待了啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊！！");
                                                    }
                                                }
                                                else if (messageContent.Contains("龙图") || messageContent.Contains("杀妈") || messageContent.Contains("龙玉涛") || messageContent.Contains("马"))
                                                {
                                                    List<Element> elements = new List<Element>();
                                                    byte[] imgBytes = FunnyImage.TryFindPhoto(FunnyImage.ImageType.Lyt);
                                                    if (imgBytes != null)
                                                    {
                                                        elements.Add(new ElementImage(imgBytes));
                                                        elements.Add(new ElementText("\n今天真是个适合杀马的日子呢，你说呢¿"));
                                                        elements.Add(new ElementAt(me.sender.user_id));
                                                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(elements.ToArray()));
                                                    }
                                                    else
                                                    {
                                                        MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "龙图库里没有数据哦~快让我有更多的马可以杀吧！！");
                                                    }
                                                }
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                case TextType.SponsorNeeded:
                                    {
                                        Command.GetSponsorImage(me);
                                        break;
                                    }
                                case TextType.LotteryNeeded:
                                    {
                                        if (GlobalObj.GroupInfoLists[me.group_id].IsLotteryEnabled)
                                        {
                                            if (GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Find(e => e == me.sender.user_id) == 0)
                                            {
                                                GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Add(me.sender.user_id);
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"【{GlobalObj.GroupInfoLists[me.group_id].CurrentLotteryName}】抽奖活动报名成功，当前共有{GlobalObj.GroupInfoLists[me.group_id].LotteryUserList.Count}人参与，请静候开奖！");
                                            }
                                            else
                                            {
                                                MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, $"您已经报名过本群活动了，请静候开奖哦~！");
                                            }
                                        }
                                        else
                                        {
                                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "目前并没有正在进行的抽奖活动哦~");
                                        }
                                        break;
                                    }
                                case TextType.RapeMember:
                                    {
                                        if (isAdmin || userAccessLevel >= 100)
                                        {
                                            string messageContent = me.message.data[0].data["text"];
                                            if (me.message.data[0].type.Equals("text", StringComparison.Ordinal))
                                            {
                                                int rapeIndex = messageContent.IndexOf("雷普");
                                                int baIndex = messageContent.IndexOf("吧");

                                                string memberName = "";
                                                if (baIndex != -1)
                                                {
                                                    memberName = messageContent[(rapeIndex + 2)..baIndex];
                                                }
                                                else
                                                {
                                                    memberName = messageContent[(rapeIndex + 2)..messageContent.Length];
                                                }
                                                //小写处理一遍成员名字
                                                memberName = memberName.ToLower();

                                                int rapeLevel = 0;
                                                if (messageContent.Contains("用力"))
                                                {
                                                    rapeLevel += 1;

                                                }
                                                if (messageContent.Contains("狠狠地"))
                                                {
                                                    rapeLevel += 1;
                                                }
                                                FunnyImage.RapeGroupMember(me.group_id, memberName, me.sender.user_id, rapeLevel);
                                            }
                                        }
                                        break;
                                    }
                                case TextType.NavySchool:
                                    {
                                        if (userAccessLevel >= 2)
                                        {
                                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                                        MessageType.group_,
                                                        me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/hyjs.amr")));

                                        }
                                        else
                                        {
                                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                                        MessageType.group_,
                                                        me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/zmxs.amr")));

                                        }
                                        break;
                                    }
                                case TextType.MusicNeeded:
                                    {
                                        string messageContent = me.message.data[0].data["text"];
                                        if (messageContent.Contains("火力精神王"))
                                        {
                                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                                        MessageType.group_,
                                                        me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/hljsw.amr")));

                                        }
                                        else if (messageContent.Contains("恶臭") || messageContent.Contains("先辈") || messageContent.Contains("野兽") || messageContent.Contains("浩二") || messageContent.Contains("田所") || messageContent.Contains("昏睡") || messageContent.Contains("红茶") || messageContent.Contains("雷普"))
                                        {
                                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                                        MessageType.group_,
                                                        me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/xianbei.amr")));

                                        }
                                        else if (messageContent.Contains("我爱中国") || messageContent.Contains("伏拉夫") || messageContent.ToLower().Contains("flf") || messageContent.Contains("处刑曲"))
                                        {
                                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                                        MessageType.group_,
                                                        me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/flf.amr")));
                                        }
                                        break;
                                    }
                                case TextType.DianNiuZiNeeded:
                                    {
                                        await GlobalObj.WSRobotClient.SendMessageAsync(
                                                    MessageType.group_,
                                                    me.group_id, new Message(new ElementRecord("https://ipdle.com/cyrustools/qqrobot/record/dianniuzi.amr")));
                                        break;
                                    }
                                default:
                                    {
                                        if (isAdmin)
                                        {
                                            Random rnds = new();
                                            string sendText = PresetsText.CyrusResponse[rnds.Next(PresetsText.CyrusResponse.Length)];

                                            await GlobalObj.WSRobotClient.SendTextAsync(
                                             MessageType.group_,
                                             me.group_id,
                                                     sendText);
                                        }
                                        else
                                        {

                                            Random rnds = new();
                                            string sendText = PresetsText.HelloResponse[rnds.Next(PresetsText.HelloResponse.Length)];
                                            await GlobalObj.WSRobotClient.SendTextAsync(MessageType.group_, me.group_id, sendText);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        User userItem = User.Find(User._.Uid == me.sender.user_id);
                        if (userItem == null)
                        {
                            Console.WriteLine($"正在更新群{me.group_id}的用户数据……");
                            List<GroupUser> groupUsers = new();
                            GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(me.group_id)).Result;
                            Console.WriteLine($"群{me.group_id}目前有{groupMemberList.memberInfo.Length}个用户");
                            string userSuperNoobId = string.Empty;
                            foreach (GroupMemberInfo userInfo in groupMemberList.memberInfo)
                            {
                                if (userInfo.user_id == me.sender.user_id)
                                {
                                    userSuperNoobId = IdentityHelper.GetUserSuperNoobId(userInfo.card);
                                }
                            }
                            if (string.IsNullOrEmpty(userSuperNoobId)) userSuperNoobId = "未知";

                            //如果数据库中没有这个用户的数据，就将该用户写入数据库
                            userItem = IdentityHelper.UserRegister(me.sender.user_id, userSuperNoobId);
                        }

                        userItem.OffenceCount += 1;

                        if (userItem.OffenceCount < 3)
                        {
                            var msgList = new List<Element>
                            {
                                new ElementFace("128116", "emoji"),
                                new ElementText("不认识你，爪巴！"),
                                new ElementAt(me.sender.user_id)
                            };
                            Random skinColorRnd = new Random();
                            int skinColor = skinColorRnd.Next(0, 6);
                            if (skinColor != 0)
                            {
                                msgList.Insert(1, new ElementFace(Convert.ToString(127994 + skinColor), "emoji"));
                            }
                            Message msg = new(msgList.ToArray());

                            await GlobalObj.WSRobotClient.SendMessageAsync(
                                MessageType.group_,
                                me.group_id, msg);
                        }
                        else
                        {
                            MessageHelper.SendGroupTextMessageWithAt(me.group_id, me.sender.user_id, "不理你啦！ヽ(#`Д´)ﾉ");

                            Thread.Sleep(200);
                            GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 36000)).Wait();
                            userItem.OffenceCount = 0;
                        }
                        userItem.Update();
                    }
                    return;
                }

                //复读功能
                if (GlobalSet.IsRepeaterEnabled && me.sender.user_id != GlobalObj.RobotQQ)
                {
                    bool imageRepeated = false;
                    string lastMessageImagePath = string.Empty;
                    string thisMessageImageHash = string.Empty;
                    if (me.message.data.Count == 1 && me.message.data[0].type.Equals("image") &&
                        GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content != null &&
                        GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content.data.Count == 1 &&
                        GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content.data[0].type.Equals("image"))
                    {
                        //上一个消息和这个消息都是单图片消息
                        lastMessageImagePath = HttpHelper.DownloadTempFile(GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content.data[0].data["file"]);
                        string thisMessageImagePath = HttpHelper.DownloadTempFile(me.message.data[0].data["file"]);
                        thisMessageImageHash = FileHashHelper.CalcFileHash(thisMessageImagePath);
                        if (FileHashHelper.CalcFileHash(lastMessageImagePath).Equals(thisMessageImageHash, StringComparison.InvariantCulture))
                        {
                            //这里不删最后一个消息的临时图片，用于后续处理鬼畜图片
                            imageRepeated = true;
                        }
                        else
                        {
                            File.Delete(lastMessageImagePath);
                        }
                        File.Delete(thisMessageImagePath);
                    }

                    GlobalObj.GroupInfoLists[me.group_id].LastMessage.Sender_Uid = me.sender.user_id;
                    if (me.message == GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content || imageRepeated)
                    {
                        //确保不会复读已经自己复读过的消息
                        if (GlobalObj.GroupInfoLists[me.group_id].LastRepeatedMessage != me.message)
                        {
                            if (GlobalObj.GroupInfoLists[me.group_id].IsLastRepeatImage &&
                            me.message.data.Count == 1 && me.message.data[0].type.Equals("image") &&
                            GlobalObj.GroupInfoLists[me.group_id].LastRepeatedMessage.data.Count == 1 && GlobalObj.GroupInfoLists[me.group_id].LastRepeatedMessage.data[0].type.Equals("image") &&
                            GlobalObj.GroupInfoLists[me.group_id].LastRepeatedImageHash.Equals(thisMessageImageHash))
                            {
                                //如果上一个复读的是单图片并且hash和这次的相等，则判定为无效复读，中止复读过程
                                GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content = me.message;
                                GlobalObj.GroupInfoLists[me.group_id].RepeatCount = 0;
                                return;
                            }
                            if (GlobalObj.GroupInfoLists[me.group_id].RepeatCount < 1)
                            {
                                Console.WriteLine($"群{me.group_id}检测到复读行为，当前复读次数为{GlobalObj.GroupInfoLists[me.group_id].RepeatCount}次");
                                GlobalObj.GroupInfoLists[me.group_id].RepeatCount++;
                            }
                            else
                            {
                                GlobalObj.GroupInfoLists[me.group_id].LastRepeatedMessage = me.message;

                                GlobalObj.GroupInfoLists[me.group_id].RepeatCount = 0;
                                if (me.message.data.Count == 1 && me.message.data[0].type.Equals("image"))
                                {
                                    Console.WriteLine("发现多次图片复读行为，正在处理鬼畜GIF中……");
                                    System.IO.FileInfo fileInfo = new(lastMessageImagePath);
                                    //500k = 512000 Bytes
                                    if (fileInfo.Length <= 512000)
                                    {
                                        var bytes = ImageProcessHelper.FunnyGIFConvert(lastMessageImagePath);
                                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_,
                                           me.group_id, new Message(new ElementImage(bytes)));
                                        Console.WriteLine("图片处理完毕！");
                                    }
                                    else
                                    {
                                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_,
                                           me.group_id, me.message);
                                        Console.WriteLine("文件过大，将不会进行鬼畜化处理！");
                                    }
                                    GlobalObj.GroupInfoLists[me.group_id].IsLastRepeatImage = true;
                                    GlobalObj.GroupInfoLists[me.group_id].LastRepeatedImageHash = FileHashHelper.CalcFileHash(lastMessageImagePath);
                                    File.Delete(lastMessageImagePath);
                                }
                                else
                                {
                                    List<string> tmpfileNameList = new();
                                    List<Element> processedMessageList = new();
                                    for (int i = 0; i < me.message.data.Count; i++)
                                    {
                                        if (me.message.data[i].type.Equals("text", StringComparison.Ordinal))
                                        {
                                            processedMessageList.Add(new ElementText(HuoXingConvert.ConvertToHuoXing(me.message.data[i].data["text"])));
                                        }
                                        else if (me.message.data[i].type.Equals("image", StringComparison.Ordinal))
                                        {
                                            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                                            string tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());

                                            HttpHelper.DownloadFile(me.message.data[i].data["file"], tmpFileName);
                                            FileStream fs = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                                            BinaryReader br = new BinaryReader(fs);
                                            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                                            fs.Close();

                                            processedMessageList.Add(new ElementImage(imgBytesIn));
                                            tmpfileNameList.Add(tmpFileName);
                                        }
                                        else
                                        {
                                            processedMessageList.Add(me.message.data[i]);
                                        }
                                    }
                                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_,
                                        me.group_id, new Message(processedMessageList.ToArray()));

                                    foreach (var item in tmpfileNameList)
                                    {
                                        File.Delete(item);
                                    }
                                    GlobalObj.GroupInfoLists[me.group_id].IsLastRepeatImage = false;

                                }
                            }
                        }
                        GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content = me.message;
                    }
                    else
                    {
                        GlobalObj.GroupInfoLists[me.group_id].LastMessage.Content = me.message;
                        GlobalObj.GroupInfoLists[me.group_id].RepeatCount = 0;
                    }
                }
            }

            return;
        }
    }
}
