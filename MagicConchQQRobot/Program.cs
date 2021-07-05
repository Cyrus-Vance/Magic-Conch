using MagicConchQQRobot.DataObjs;
using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.AccountInfoProvider;
using MagicConchQQRobot.Modules.InteractiveProvider;
using MagicConchQQRobot.Modules.QueryProvider.Others;
using MagicConchQQRobot.Modules.SecretProvider;
using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Clients;
using CyrusVance.CoolQRobot.Events.CQEvents;
using CyrusVance.CoolQRobot.Events.CQEvents.Base;
using CyrusVance.CoolQRobot.Events.CQResponses;
using CyrusVance.CoolQRobot.Events.CQResponses.Base;
using CyrusVance.CoolQRobot.Utils;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebPWrapper;

namespace MagicConchQQRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            XTrace.UseConsole();

            SecretData.LoadSecretData();
            DBAccess.InitDataBase();

            FunnyImage.CreateDirectory();

            WebPExecuteDownloader.Download();

            Task loginTask = new(() => UovzVPN.Login());
            loginTask.Start();

            WebImage.InitSauceNaoClients();

            GlobalObj.WSRobotClient = new CQWebsocketClient(
                   access_url: SecretData.GetSectionData("QQRobotWebSocket:access_url"),
                   access_token: SecretData.GetSectionData("QQRobotWebSocket:access_token"),
                   event_url: SecretData.GetSectionData("QQRobotWebSocket:event_url")
               );
            while (!GlobalObj.WSRobotClient.Initiated)
            {
                Thread.Sleep(1);
            }

            GlobalObj.RobotQQ = GlobalObj.WSRobotClient.SelfId;

            Console.WriteLine(
                $"QQ:{GlobalObj.RobotQQ},昵称:{GlobalObj.WSRobotClient.SelfNick},是否为pro版本:{GlobalObj.WSRobotClient.IsPro}"
            );
            Console.WriteLine("==========================");
            Console.WriteLine(" ");

            GroupData.GetGroupInfoFromSecrets();

            Task getMemberListTask = new(() => GetGroupUsers());
            getMemberListTask.Start();
            getMemberListTask.Wait();
            foreach (var currentMemberList in GlobalObj.GroupMemberLists)
            {
                List<GroupUser> groupUsers = new List<GroupUser>();
                foreach (GroupMemberInfo userInfo in currentMemberList.Value.memberInfo)
                {
                    Regex rgx = new(@"(?<=\u3010)\S+?(?=\u3011)");
                    Match match = rgx.Match(userInfo.card);

                    GroupUser user = new()
                    {
                        QQNumber = userInfo.user_id,
                        GroupNickname = userInfo.card,
                        Nickname = userInfo.nickname
                    };

                    if (match.Success)
                    {
                        user.SuperNoobID = match.Groups[0].Value;
                    }
                    groupUsers.Add(user);
                }
                GlobalObj.GroupRobotUserLists.Add(currentMemberList.Key, groupUsers);
                //Console.WriteLine(currentMemberList.Value.raw_data);
                Console.WriteLine($"群{currentMemberList.Key}共有{GlobalObj.GroupRobotUserLists[currentMemberList.Key].Count}个群内用户");

                StringBuilder informText = new StringBuilder("队内成员：\n==========================");
                foreach (var aa in GlobalObj.GroupRobotUserLists[currentMemberList.Key])
                {
                    informText.Append($"\n队员代号：{(string.IsNullOrEmpty(aa.SuperNoobID) ? "弱子" : aa.SuperNoobID)},身份：{ aa.QQNumber}");
                }
                Console.WriteLine(informText.ToString());
                Console.WriteLine("===============================");
            }

            GlobalObj.WSRobotClient.OnEventAsync += HandleEventAsync;
            while (true)
            {
                Thread.Sleep(1);
            }
        }

        private static void GetGroupUsers()
        {
            foreach (var groupId in GroupData.GetWatchGroups())
            {
                var groupInfo = (GetGroupInfoResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupInfoRequest(groupId)).Result;
                GetGroupMemberListResult groupMemberList = (GetGroupMemberListResult)GlobalObj.WSRobotClient.SendRequestAsync(new GetGroupMemberListRequest(groupId)).Result;
                GlobalObj.GroupMemberLists.Add(groupId, groupMemberList);
                GlobalObj.GroupInfoLists.Add(groupId, new GroupInfo() { GroupName = groupInfo.GroupName });
            }
        }

        private static async Task<CQResponse> HandleEventAsync(CQApiClient api, CQEvent e)
        {
            Console.WriteLine($"收到类型为{e.GetType()}的请求");
            // 处理群消息
            if (e is GroupMessageEvent)
            {
                var me = e as GroupMessageEvent;

                await MessageProcess.ProcessGroupMessageAsync(me);

            }
            return new EmptyResponse();

        }


    }
}
