using CyrusVance.CoolQRobot.Enums;

namespace CyrusVance.CoolQRobot.Events.MetaEvents {
    /// <summary>
    /// 元事件
    /// </summary>
    public class MetaEvent : CyrusVance.CoolQRobot.Events.CQEvents.Base.CQEvent {
        /// <summary></summary>
        public MetaEvent (long time, PostType postType) : base (time, postType) { }
    }
}