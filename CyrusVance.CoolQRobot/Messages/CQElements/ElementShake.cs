using CyrusVance.CoolQRobot.Messages.CQElements.Base;

namespace CyrusVance.CoolQRobot.Messages.CQElements {
    /// <summary>
    /// 戳一戳/窗口抖动
    /// </summary>
    public class ElementShake : Element {
        /// <summary></summary>
        public ElementShake () : base ("shake") { }
    }
}