using CyrusVance.CoolQRobot.ApiCall.Requests.Base;
using Newtonsoft.Json;

namespace CyrusVance.CoolQRobot.ApiCall.Requests {
    /// <summary>
    /// 用于清空插件的日志文件。
    /// </summary>
    [JsonObject]
    public class CleanDataDirRequest : ApiRequest {
        [JsonProperty] string data_dir;
        ///
        public CleanDataDirRequest (string data_dir) : base ("/clean_data_dir") {
            this.response = new Results.EmptyResult ();
            this.data_dir = data_dir;
        }
    }
}