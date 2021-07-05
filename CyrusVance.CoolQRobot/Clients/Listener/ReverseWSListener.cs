using System;
using System.Threading.Tasks;
using CyrusVance.CoolQRobot.Clients.WebsocketUtils;
using CyrusVance.CoolQRobot.Events.CQResponses;
using CyrusVance.CoolQRobot.Events.CQResponses.Base;

namespace CyrusVance.CoolQRobot.Clients.Listeners {
    class ReverseWSListener : Listener {
        public Callers.ICaller caller;
        WebsocketDaemon.WebsocketServerInstance server = null;

        public ReverseWSListener (int bind_port, string event_path, string access_token) {
            event_path = event_path.Trim ('/');
            server = new WebsocketDaemon.WebsocketServerInstance (
                bind_port, event_path, access_token,
                message => {
                    Task.Run (async () => {
                        try {
                            if (string.IsNullOrEmpty (message))
                                return;
                            CQResponse response = await GetResponse (message);
                            if (response is EmptyResponse == false) {
                                if (caller != null)
                                    await caller.SendRequestAsync (
                                        new ApiCall.Requests.HandleQuickOperationRequest (
                                            context: message,
                                            operation: response.content
                                        ));
                                else
                                    Log.Warn ("反向WS无法快速响应（caller为null）");
                            }
                        } catch (System.Exception e) {
                            Log.Error (
                                $"处理事件时发生未处理的异常{e},错误信息为{e.Message}"
                            );
                        }
                    });
                }
            );
        }
    }
}