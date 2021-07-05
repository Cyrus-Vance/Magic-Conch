using Newtonsoft.Json.Linq;
using System;

namespace CyrusVance.CoolQRobot.ApiCall.Results
{
    /// <summary>
    /// 获取群消息请求结果
    /// </summary>
    public class GetGroupInfoResult : Base.ApiResult
    {
        /// <summary>
        /// 群号
        /// </summary>
        public long GroupId { get; private set; }

        /// <summary>
        /// 群名称
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// 群成员数
        /// </summary>
        public int MemberCount { get; private set; }

        /// <summary>
        /// 最大成员数
        /// </summary>
        public int MaxMemberCount { get; private set; }

        ///
        public override void Parse(JToken result)
        {
            PreCheck(result);
            try
            {
                GroupId = Convert.ToInt64(raw_data["group_id"]);
                GroupName = raw_data["group_name"].ToString();
                MemberCount = Convert.ToInt32(raw_data["member_count"]);
                MaxMemberCount = Convert.ToInt32(raw_data["max_member_count"]);
            }
            catch
            {
                Log.Error("调用发送消息API未返回message_id");
                throw new Exceptions.ErrorApicallException();
            }
        }
    }
}
