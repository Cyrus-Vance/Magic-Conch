namespace CyrusVance.CoolQRobot.Clients {
    /// <summary>websocket协议调用api</summary>
    public class CQWebsocketClient : CQApiClient {
        /// <summary></summary>
        public CQWebsocketClient (
            string access_url,
            string access_token = "",
            string event_url = "",
            bool use_group_table = false,
            bool use_message_table = false
        ) : base (
            new Callers.WebsocketCaller (access_url, access_token),
            new Listeners.WebsocketListener (event_url, access_token),
            use_group_table, use_message_table
        ) {
            (listener as Listeners.WebsocketListener).caller = caller;
            listener.RegisterHandler (HandleEvent);
            initiate_task = System.Threading.Tasks.Task.Run(Initiate);
            initiate_task.Wait();
        }
    }
}