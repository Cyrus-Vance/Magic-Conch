namespace MagicConchQQRobot.Presets
{
    class HelpText
    {
        public static string Wrong_Format = "您输入的格式有误，请检查您的格式！\n" +
                                    "============================\n";

        // 参数0：命令说明，参数1：中文指令名，参数2：英文指令名，参数3：指令参数，参数4：指令示例
        public static string Format_Example = "{0} 格式：\n" +
                                    "【呼叫神奇海螺】{1} {3}\n" +
                                    "（中间有英文空格间隔）\n" +
                                    "【例】神奇海螺，{1} {4}\n" +
                                    "您也可以使用快捷指令：/{2} {3}来实现！\n" +
                                    "【例】/{2} {4}";

        private static string GetCommandHelpText(bool isWrongFormat, string commandDesc, string chnCmdName, string engCmdName, string cmdParams, string cmdExample)
        {
            string generatedStr = string.Empty;
            if (isWrongFormat)
            {
                generatedStr = Wrong_Format;
            }
            generatedStr += string.Format(Format_Example, commandDesc, chnCmdName, engCmdName, cmdParams, cmdExample);
            return generatedStr;
        }

        public static string Wrong_CommandUse = "您输入的命令格式有误，请检查后再输入！";

        public static string FFXIV_Savage(bool isWrongFormat)
        {
            return GetCommandHelpText(isWrongFormat, "查询国服零式通关日期", "查询国服零式", "savage", "[玩家名] [服务器名]", "丝瓜卡夫卡 幻影群岛");
        }

        public static string PCR_Rank(bool isWrongFormat)
        {
            return GetCommandHelpText(isWrongFormat, "查询公主连结Rank图", "查询rank", "rank", "[日/台/国]", "台");
        }

    }
}
