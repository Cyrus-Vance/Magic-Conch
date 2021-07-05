using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrusVance.CoolQRobot.ApiCall.Requests
{
    /// <summary>
    /// 获取指定群的信息
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    public class GetGroupInfoRequest : Base.ApiRequest
    {
        [JsonProperty]long group_id;
        [JsonProperty]bool no_cache;
        /// <summary>
        /// 获取指定群的信息请求
        /// </summary>
        public GetGroupInfoRequest(long group_id, bool no_cache = false) : base("/get_group_info")
        {
            response = new Results.GetGroupInfoResult();
            this.group_id = group_id;
            this.no_cache = no_cache;
        }
    }
}
