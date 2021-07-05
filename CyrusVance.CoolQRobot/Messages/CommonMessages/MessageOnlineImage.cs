using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;

namespace CyrusVance.CoolQRobot.Messages.CommonMessages {
    /// <summary>
    /// 在线图片
    /// </summary>
    public class MessageOnlineImage : Message {
        /// <param name="url">图片的url</param>
        /// <param name="useCache">是否缓存</param>
        public MessageOnlineImage (string url, bool useCache = true):
            base (new ElementImage (url, useCache)) { }
    }
}