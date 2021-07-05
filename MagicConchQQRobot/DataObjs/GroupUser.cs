using System;
using System.Collections.Generic;

namespace MagicConchQQRobot.DataObjs
{
    class GroupUser
    {
        /// <summary>
        /// QQ号码
        /// </summary>
        public long QQNumber { get; set; }

        /// <summary>
        /// 群内的昵称
        /// </summary>
        public string GroupNickname { get; set; }

        /// <summary>
        /// QQ昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// SuperNoob队内的代号
        /// </summary>
        public string SuperNoobID { get; set; }

        /// <summary>
        /// 识图功能的激活状态，0为关闭，1为全站识别，2为Pixiv Only
        /// </summary>
        public int ImageIdentityNeededMode { get; set; }

        /// <summary>
        /// 图片识别的激活状态，1为粪图，2为龙图
        /// </summary>
        public int ImageLearnNeededMode { get; set; }

        /// <summary>
        /// 用户需要雷普的用户列表
        /// </summary>
        public List<GroupUser> RapeUserList { get; set; }

        /// <summary>
        /// 用户雷普人的等级
        /// </summary>
        public int RapeLevel { get; set; }
    }
}
