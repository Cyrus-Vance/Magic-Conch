using MagicConchQQRobot.Modules.SecretProvider;
using System;
using System.Collections.Generic;

namespace MagicConchQQRobot.Modules.AccountInfoProvider
{
    public static class GroupData
    {
        private static List<long> SpecialServiceGroups;
        private static List<long> WatchGroups;

        public static void GetGroupInfoFromSecrets()
        {
            SpecialServiceGroups = new List<long>();
            WatchGroups = new List<long>();

            foreach (var item in SecretData.GetChildren("Groups:SpecialServiceGroup"))
            {
                SpecialServiceGroups.Add(item.Value.ToLong());
            }
            string watchGroupPath;
#if DEBUG
            watchGroupPath = "Groups:WatchGroupList-Dev";
#else
            watchGroupPath = "Groups:WatchGroupList";
#endif
            foreach (var item in SecretData.GetChildren(watchGroupPath))
            {
                WatchGroups.Add(item.Value.ToLong());
            }
        }

        public static List<long> GetSpecialServiceGroups()
        {
            return SpecialServiceGroups;
        }

        public static List<long> GetWatchGroups()
        {
            return WatchGroups;
        }
    }
}
