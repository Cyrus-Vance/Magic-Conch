using MagicConchQQRobot.Globals;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;

namespace MagicConchQQRobot.Modules.Utils
{
    class MessageHelper
    {
        public static void SendGroupTextMessage(long groupId, string content)
        {
            GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId,
                    new Message(new ElementText(content))).Wait();
        }

        public static void SendGroupTextMessageWithAt(long groupId, long sendId, string content)
        {
            GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId,
                    new Message(
                        new ElementAt(sendId),
                        new ElementText(content))).Wait();
        }
    }
}
