using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;

namespace CyrusVance.CoolQRobot.Messages.CommonMessages {
    /// <summary>
    /// 在群组中发送匿名消息
    /// </summary>
    public class MessageAnnonymous : Message {
        /// <param name="ignore">是否强制发送</param>
        /// <param name="dict"></param>
        public MessageAnnonymous (bool ignore, params Element[] dict):
            base () {
                this.data = new System.Collections.Generic.List<Element> ();
                this.data.Add (new ElementAnnonymous (ignore));
                this.data.AddRange (dict);
            }
    }
}