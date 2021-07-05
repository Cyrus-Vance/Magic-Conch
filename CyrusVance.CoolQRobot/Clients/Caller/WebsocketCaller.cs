using CyrusVance.CoolQRobot.ApiCall.Requests.Base;
using CyrusVance.CoolQRobot.ApiCall.Results.Base;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CyrusVance.CoolQRobot.Clients.Callers
{
    internal class WebsocketCaller : ICaller
    {
        private readonly string access_url;
        private readonly string access_token;
        ClientWebSocket client = new ClientWebSocket();
        CancellationTokenSource token_source = new CancellationTokenSource();

        public WebsocketCaller(string access_url, string access_token)
        {
            if (access_url.EndsWith("/api"))
            {
                //access_url += '/';
            }
            else if (!access_url.EndsWith("/api/"))
            {
                access_url += "/api";
            }
            this.access_url = access_url;
            this.access_token = access_token;
        }
        public async Task Reconnect()
        {
            client.Abort();
            client = new ClientWebSocket();
            await client.ConnectAsync(
                new Uri(access_url + (access_token == "" ? "" : "?access_token=" + access_token)),
                token_source.Token
            );
        }
        async Task SendText(string text)
        {
            await client.SendAsync(
                Encoding.UTF8.GetBytes(text),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                token_source.Token
            );
        }
        async Task<string> ReceiveText()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await client.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                using var reader = new StreamReader(ms, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            else
            {
                return null;
            }


            /*StringBuilder result = new StringBuilder();
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
            while (true)
            {
                WebSocketReceiveResult res = await client.ReceiveAsync(
                        buffer: buffer,
                        cancellationToken: new CancellationToken()
                    );
                result.Append(Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
                if (res.EndOfMessage) break;
            }
            return result.ToString();*/
        }
        SemaphoreSlim lock_ = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public async Task<ApiResult> SendRequestAsync(ApiRequest x)
        {
            var wait = lock_.WaitAsync();
            try
            {
                JObject constructor = new JObject
                {
                    ["action"] = x.api_path.Substring(1),
                    ["params"] = JObject.Parse(x.content),
                    ["echo"] = System.DateTime.Now.Millisecond
                };
                await wait;
                if (client.State != WebSocketState.Open)
                    await Reconnect();
                await SendText(constructor.ToString(Newtonsoft.Json.Formatting.None));
                var resp = await ReceiveText();
                if (resp.Contains("authorization failed"))
                    throw new Exceptions.ErrorApicallException("access token 有误");
                x.response.Parse(JToken.Parse(resp));
            }
            catch (System.Exception e)
            {
                Log.Error("Websocket调用API失败");
                Log.Debug("调用:" + x.ToString());
                Log.Debug("Traceback:" + e.StackTrace);
                throw new Exceptions.NetworkFailureException("调用API失败");
            }
            finally
            {
                lock_.Release();
            }
            return x.response;
        }
    }
}