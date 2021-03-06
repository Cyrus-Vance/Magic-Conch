using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using CyrusVance.CoolQRobot.Events.CQResponses;
using CyrusVance.CoolQRobot.Events.CQResponses.Base;

namespace CyrusVance.CoolQRobot.Clients.Listeners {
    class WebsocketListener : Listener {
        ClientWebSocket client;
        public Callers.ICaller caller;
        CancellationTokenSource ctoken_source;
        public WebsocketListener (string event_url, string access_token) {

                event_url=event_url.Replace("/event/", "/event");
            event_url += !string.IsNullOrEmpty (access_token) ? "?access_token=" + access_token : "";

            client = new ClientWebSocket ();
            ctoken_source = new CancellationTokenSource ();
            Task.Run (async () => {
                byte[] recv_buffer = new byte[2048];
                List<byte> message = new List<byte> ();
                while (ctoken_source.Token.IsCancellationRequested == false) {
                    await client.ConnectAsync (
                        new System.Uri (event_url),
                        ctoken_source.Token
                    );
                    while (
                        client.State == WebSocketState.Open &&
                        ctoken_source.Token.IsCancellationRequested == false
                    ) {
                        var t = await client.ReceiveAsync (
                            recv_buffer,
                            ctoken_source.Token
                        );
                        if (!t.EndOfMessage) {
                            message.AddRange (recv_buffer);
                        } else {
                            message.AddRange (recv_buffer.ToList ().GetRange (0, t.Count));
                            Process (System.Text.Encoding.UTF8.GetString (message.ToArray ()));
                            message.Clear ();
                        }
                    }
                }
            });
        }

        async void Process (string message) {
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
                        Log.Warn ("WS?????????????????????caller???null???");
                }
            } catch (System.Exception e) {
                Log.Error ($"???????????????????????????????????????{e},???????????????{e.Message}");
            }
        }
        ~WebsocketListener () {
            ctoken_source.Cancel ();
            client.Dispose ();
        }
    }
}