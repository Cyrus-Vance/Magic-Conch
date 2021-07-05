using System.Collections.Generic;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;

namespace CyrusVance.CoolQRobot.Messages.CQElements {
    /// <summary>
    /// 文本消息段
    /// </summary>
    public class ElementText : Element {
        ///
        public string text { get; private set; }
        /// <summary></summary>
        public ElementText (string text):
            base ("text", ("text", text)) { this.text = text; }
    }
}