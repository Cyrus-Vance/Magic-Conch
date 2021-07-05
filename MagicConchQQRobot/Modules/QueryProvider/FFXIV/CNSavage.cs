using MagicConchQQRobot.DataObjs.DbClass;
using MagicConchQQRobot.Modules.Utils;
using System.Collections.Generic;

namespace MagicConchQQRobot.Modules.QueryProvider.FFXIV
{
    class CNSavage
    {
        /// <summary>
        /// 查询玩家零式攻略状态
        /// </summary>
        /// <param name="playerName">玩家角色名</param>
        /// <param name="serverName">玩家所属服务器名（中文）</param>
        public static string CheckPlayerCNSavage(string playerName, string serverName,long groupId)
        {

            IList<FfxivGameserver> gameserverList = FfxivGameserver.FindAll(FfxivGameserver._.ServerName == serverName);

            if (gameserverList.Count > 0)
            {
                FfxivGameserver gameserver = gameserverList[0];
                LogHelper.Debug(groupId,$"开始查询{playerName}的国服零式数据，输入服务器名为{serverName}，查询出AreaId为{gameserver.AreaID}，GroupId为{gameserver.GroupID}");
                string returnJson = HttpHelper.HttpPost(@"https://actff1.web.sdo.com/20180525HeroList/Server/HeroList190128.ashx", @$"method=queryhreodata&Stage=2&Name={playerName}&AreaId={gameserver.AreaID}&GroupId={gameserver.GroupID}");
                return returnJson;
            }
            else
            {
                LogHelper.Debug(groupId,$"玩家名：{playerName}，服务器名：{serverName}，查询的国服零式数据中，没有发现服务器数据！");
                return null;
            }

        }

    }
}
