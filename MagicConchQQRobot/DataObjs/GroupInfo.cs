using CyrusVance.CoolQRobot.Messages;
using System.Collections.Generic;

namespace MagicConchQQRobot.DataObjs
{
    class GroupInfo
    {
        public GroupInfo()
        {
            LastMessage = new LastMessage();
            LotteryUserList = new List<long>();
        }

        public string GroupName { get; set; }

        public LastMessage LastMessage { get; set; }

        public Message LastRepeatedMessage { get; set; }

        public bool IsLastRepeatImage { get; set; }

        public string LastRepeatedImageHash { get; set; }

        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// 复读次数计数
        /// </summary>
        public int RepeatCount { get; set; }

        /// <summary>
        /// 是否开启了抽奖活动
        /// </summary>
        public bool IsLotteryEnabled { get; set; }

        /// <summary>
        /// 当前抽奖活动的名字
        /// </summary>
        public string CurrentLotteryName { get; set; }

        /// <summary>
        /// 当前抽奖活动的名字
        /// </summary>
        public List<long> LotteryUserList { get; set; }
    }

    class LastMessage
    {
        public Message Content { get; set; }
        public long Sender_Uid { get; set; }
    }
}
