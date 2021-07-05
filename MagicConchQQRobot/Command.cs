using MagicConchQQRobot.Globals;
using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Events.CQEvents;
using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MagicConchQQRobot
{
    class Command
    {
        public static async Task QueryFFXIVAsync(GroupMessageEvent me, string userName, string serverName)
        {
            string savageJsonText = Modules.QueryProvider.FFXIV.CNSavage.CheckPlayerCNSavage(userName, serverName, me.group_id);
            if (!string.IsNullOrEmpty(savageJsonText))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(savageJsonText);

                string[] passTime = new string[4];
                passTime[0] = jo["Attach"]["Level1"].ToString();
                passTime[1] = jo["Attach"]["Level2"].ToString();
                passTime[2] = jo["Attach"]["Level3"].ToString();
                passTime[3] = jo["Attach"]["Level4"].ToString();
                Console.WriteLine($"lv1:{passTime[0]},lv2:{passTime[1]},lv3:{passTime[2]},lv4:{passTime[3]}");
                await GlobalObj.WSRobotClient.SendMessageAsync(
                    MessageType.group_,
                    me.group_id,
                            new Message(
                                new ElementText($"冒险者 {userName} - {serverName} 通关本期零式的日期为："
                                     + $"\n零式一层：{(string.IsNullOrEmpty(passTime[0]) ? "未攻破" : $"{passTime[0].Substring(0, 4)}年{passTime[0].Substring(4, 2).ToInt()}月{passTime[0].Substring(6, 2).ToInt()}日")}"
                                     + $"\n零式二层：{(string.IsNullOrEmpty(passTime[1]) ? "未攻破" : $"{passTime[1].Substring(0, 4)}年{passTime[1].Substring(4, 2).ToInt()}月{passTime[1].Substring(6, 2).ToInt()}日")}"
                                     + $"\n零式三层：{(string.IsNullOrEmpty(passTime[2]) ? "未攻破" : $"{passTime[2].Substring(0, 4)}年{passTime[2].Substring(4, 2).ToInt()}月{passTime[2].Substring(6, 2).ToInt()}日")}"
                                     + $"\n零式四层：{(string.IsNullOrEmpty(passTime[3]) ? "未攻破" : $"{passTime[3].Substring(0, 4)}年{passTime[3].Substring(4, 2).ToInt()}月{passTime[3].Substring(6, 2).ToInt()}日")}"
                                     + "\n"),
                                new ElementAt(me.sender.user_id)));
            }
        }

        /// <summary>
        /// 查询PCR黄骑充电的情况
        /// </summary>
        /// <param name="me"></param>
        /// <param name="RegionID"></param>
        /// <param name="isRefreshNeeded"></param>
        /// <returns></returns>
        public static async Task QueryHQCDAsync(GroupMessageEvent me, bool isRefreshNeeded = false)
        {
            string HQCDImgPath = "https://ipdle.com/cyrustools/qqrobot/image/黄骑充电.jpg";

            if (!isRefreshNeeded)
            {
                await GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 60));
                await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(HQCDImgPath, !isRefreshNeeded), new ElementText("\n大圈是1动充电对象 PvP测试\n黄骑四号位例外较多\n对面羊驼或中后卫坦 有可能歪\n我方羊驼算一号位"),new ElementAt(me.sender.user_id)));
            }
            else
            {
                await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("尊敬的Master，图片缓存已更新！以下是最新的图片：")));
                await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(HQCDImgPath, !isRefreshNeeded)));
            }

        }

        public static async Task<bool> QueryPCRRankAsync(GroupMessageEvent me, int RegionID, bool isRefreshNeeded = false)
        {
            switch (RegionID)
            {
                case 1:
                    string JRank1ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/日rank-1.png";
                    string JRank2ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/日rank-2.png";
                    string JRank3ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/日rank-3.png";

                    if (!isRefreshNeeded)
                    {
                        await GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 60));
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("表格仅供参考，搬运自网络，图源见图中标识\n日服R17-3 rank表：")));
                    }
                    else
                    {
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("尊敬的Master，图片缓存已更新！以下是最新的图片：")));
                    }

                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(JRank1ImgPath, !isRefreshNeeded)));
                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(JRank2ImgPath, !isRefreshNeeded)));
                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(JRank3ImgPath, !isRefreshNeeded)));

                    break;
                case 2:
                    string TRank1ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/台rank-1.png";
                    string TRank2ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/台rank-2.png";
                    string TRank3ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/台rank-3.png";

                    if (!isRefreshNeeded)
                    {
                        await GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 60));
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("表格仅供参考，搬运自网络，图源见图中标识\n台服4月 rank表：")));
                    }
                    else
                    {
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("尊敬的Master，图片缓存已更新！以下是最新的图片：")));
                    }

                    //FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Image/台rank-1.png", FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                    //BinaryReader br = new BinaryReader(fs);
                    //byte[] imgBytesIn1 = br.ReadBytes((int)fs.Length); //将流读入到字节数组中

                    //fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Image/台rank-2.png", FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                    //br = new BinaryReader(fs);
                    //byte[] imgBytesIn2 = br.ReadBytes((int)fs.Length); //将流读入到字节数组中

                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(TRank1ImgPath, !isRefreshNeeded)));
                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(TRank2ImgPath, !isRefreshNeeded)));
                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(TRank3ImgPath, !isRefreshNeeded)));
                    break;
                case 3:
                    string BRank1ImgPath = "https://ipdle.com/cyrustools/qqrobot/image/国rank.jpg";

                    if (!isRefreshNeeded)
                    {
                        await GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(me.group_id, me.sender.user_id, 60));
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("表格仅供参考，搬运自网络，图源见图中标识\n国服rank表：")));
                    }
                    else
                    {
                        await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementAt(me.sender.user_id), new ElementText("尊敬的Master，图片缓存已更新！以下是最新的图片：")));
                    }

                    await GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id, new Message(new ElementImage(BRank1ImgPath, !isRefreshNeeded)));
                    break;
                default:
                    return false;
            }
            return true;
        }
        
        public static void GetSponsorImage(GroupMessageEvent me)
        {
            const string AlipaySponsorUrl = "https://ipdle.com/cyrustools/qqrobot/image/alipay.jpg";
            const string WechatSponsorUrl = "https://ipdle.com/cyrustools/qqrobot/image/wechat.png";

            List<Element> elements = new List<Element>
            {
                new ElementText("如果需要赞助CV，您可以通过以下途径进行自愿的赞助哦~感谢您的支持！\n======================\n"),
                new ElementText("支付宝：\n"),
                new ElementImage(AlipaySponsorUrl),
                new ElementText("\n微信：\n"),
                new ElementImage(WechatSponsorUrl),
                new ElementText("\n您的每一笔赞助都将推动SuperNoob向更好的方向发展！")
            };
            GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, me.group_id,new Message(elements.ToArray())).GetAwaiter().GetResult();
        }
    }
}
