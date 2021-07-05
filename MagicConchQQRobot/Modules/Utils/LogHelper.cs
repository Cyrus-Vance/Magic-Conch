using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.SecretProvider;
using System;

namespace MagicConchQQRobot.Modules.Utils
{
    class LogHelper
    {
        /// <summary>
        /// 发送调试消息
        /// </summary>
        /// <param name="text">要发送的文本</param>
        public static void Debug(long groupId, string text)
        {
            Console.WriteLine($@"[{DateTime.Now:HH:mm:ss}][调试]{text}");
            if (GlobalObj.GroupInfoLists[groupId].IsDebugEnabled) MessageHelper.SendGroupTextMessage(SecretData.GetSectionData("Groups:DebugGroup").ToLong(), "[调试]" + text);
        }
    }
}
