using CyrusVance.CoolQRobot.ApiCall.Requests.Base;

namespace CyrusVance.CoolQRobot.ApiCall.Requests {
    /// <summary>
    /// 用于清空插件的日志文件。
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    public class CleanPluginLogRequest : ApiRequest {
        ///
        public CleanPluginLogRequest () : base ("/clean_plugin_log") {
            this.response = new Results.EmptyResult ();
        }
    }
}