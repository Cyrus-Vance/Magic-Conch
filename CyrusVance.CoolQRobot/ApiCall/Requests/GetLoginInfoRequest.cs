using CyrusVance.CoolQRobot.ApiCall.Results;

namespace CyrusVance.CoolQRobot.ApiCall.Requests {
    /// <summary></summary>
    [Newtonsoft.Json.JsonObject]
    public class GetLoginInfoRequest : Base.ApiRequest {
        /// <summary></summary>
        public GetLoginInfoRequest () : base ("/get_login_info") {
            this.response = new GetLoginInfoResult ();
        }
    }
}