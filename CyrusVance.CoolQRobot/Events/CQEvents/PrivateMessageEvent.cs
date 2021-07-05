using CyrusVance.CoolQRobot.Events.CQEvents.Base;
using CyrusVance.CoolQRobot.Messages;
using Newtonsoft.Json.Linq;

namespace CyrusVance.CoolQRobot.Events.CQEvents {
    /// <summary></summary>
    public class PrivateMessageEvent : MessageEvent {
        /// <summary></summary>
        public long sender_id { get; private set; }
        /// <summary></summary>
        public PrivateMessageEvent (
                long time,
                Message message,
                Sender sender,
                int message_id
            ):
            base (time, Enums.MessageType.private_, sender, message, message_id) {
                this.sender_id = sender.user_id;
            }
    }
}