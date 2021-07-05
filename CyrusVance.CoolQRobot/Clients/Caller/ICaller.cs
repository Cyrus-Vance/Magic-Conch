using System.Threading.Tasks;
using CyrusVance.CoolQRobot.ApiCall.Requests.Base;
using CyrusVance.CoolQRobot.ApiCall.Results.Base;

namespace CyrusVance.CoolQRobot.Clients.Callers {
    /// <summary>
    /// 
    /// </summary>
    public interface ICaller {
        /// <summary>
        /// 调用API
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        Task<ApiResult> SendRequestAsync (ApiRequest request);
    }
}