using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Events.CQEvents.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrusVance.CoolQRobot.Events.CQEvents
{
    /// <summary>
    /// 群员消息被管理员撤回
    /// </summary>
    public class GroupRecallEvent : GroupNoticeEvent
    {
        ///
        public GroupRecallEvent(
                long time, bool is_ban,
                long group_id, long operator_id, long message_id, long user_id,
                long duration) :
            base(time, NoticeType.group_recall, group_id, user_id)
        {

        }
    }
}
