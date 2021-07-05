using CyrusVance.CoolQRobot.Enums;

namespace CyrusVance.CoolQRobot.Events.CQEvents {
    class UnknownEvent : Base.CQEvent {
        public string raw_event { get; private set; }
        public UnknownEvent (long time, PostType postType, string raw):
            base (time, postType) {
                this.raw_event = raw;
            }
    }
}