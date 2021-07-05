using CyrusVance.CoolQRobot.Clients.Callers;
using CyrusVance.CoolQRobot.Clients.Listeners;

namespace CyrusVance.CoolQRobot.Clients {
    /// <summary>以HTTP协议调用API</summary>
    public class CQHTTPClient : CQApiClient {
        /// <summary></summary>
        public CQHTTPClient (
            string access_url,
            string access_token = "",
            int listen_port = -1,
            string secret = "",
            bool use_group_table = false,
            bool use_message_table = false
        ) : base (
            new HTTPCaller (access_url, access_token),
            new HTTPListener (listen_port, secret),
            use_group_table, use_message_table) {
            if (listen_port != -1) {
                this.listener.RegisterHandler (HandleEvent);
                Log.Info ($"开始在{listen_port}端口上监听上报消息");
            }
            initiate_task = System.Threading.Tasks.Task.Run(Initiate);
            initiate_task.Wait();
        }
    }

}