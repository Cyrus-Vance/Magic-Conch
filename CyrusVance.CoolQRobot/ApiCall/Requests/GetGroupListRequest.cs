namespace CyrusVance.CoolQRobot.ApiCall.Requests
{
    /// <summary>
    /// 获取加入的群列表
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    public class GetGroupListRequest : Base.ApiRequest
    {

        /// <summary>
        /// 获取群组列表请求
        /// </summary>
        public GetGroupListRequest() : base("/get_group_list")
        {
            response = new Results.GetGroupListResult();
        }
    }
}