using CyrusVance.CoolQRobot.Events.CQEvents.Base;
using CyrusVance.CoolQRobot.Messages;
using Newtonsoft.Json.Linq;

namespace CyrusVance.CoolQRobot.Events.CQEvents {
    /// <summary></summary>
    public class DiscussMessageEvent : MessageEvent {
        /// <summary>讨论组编号，只能从此处获取</summary>
        public long discuss_id { get; private set; }
        /// <summary></summary>
        public DiscussMessageEvent (
                long time,
                Message message,
                Sender sender,
                int message_id,
                long discuss_id
            ):
            base (time, Enums.MessageType.group_, sender, message, message_id) {
                this.discuss_id = discuss_id;
            }
    }
}