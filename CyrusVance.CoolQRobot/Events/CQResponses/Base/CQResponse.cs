using Newtonsoft.Json;
namespace CyrusVance.CoolQRobot.Events.CQResponses.Base {
    /// <summary></summary>
    public abstract class CQResponse {

        /// <summary></summary>
        [JsonIgnore]
        public virtual string content {
            get {
                return JsonConvert.SerializeObject (this);
            }
        }
        /// <summary></summary>
        public override string ToString () {
            return this.content;
        }
    }
}