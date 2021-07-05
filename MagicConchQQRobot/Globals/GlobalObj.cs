using MagicConchQQRobot.DataObjs;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Clients;
using System.Collections.Generic;
using System.Net;

namespace MagicConchQQRobot.Globals
{
    class GlobalObj
    {
        public static long RobotQQ = 0;
        public static CQWebsocketClient WSRobotClient;
        public static Dictionary<long, GetGroupMemberListResult> GroupMemberLists = new();
        public static Dictionary<long, List<GroupUser>> GroupRobotUserLists = new();
        public static Dictionary<long, GroupInfo> GroupInfoLists = new();
        public static CookieContainer GlobalCookies = new();
    }
}
