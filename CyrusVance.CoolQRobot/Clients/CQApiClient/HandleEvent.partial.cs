using System.Threading.Tasks;
using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Events.CQEvents;
using CyrusVance.CoolQRobot.Events.CQEvents.Base;
using CyrusVance.CoolQRobot.Events.CQResponses;
using CyrusVance.CoolQRobot.Events.CQResponses.Base;
using CyrusVance.CoolQRobot.Events.MetaEvents;
using CyrusVance.CoolQRobot.Utils;

namespace CyrusVance.CoolQRobot.Clients {
    /// <summary></summary>
    public partial class CQApiClient {
        /// <summary></summary>
        public delegate CQResponse OnEventDelegate (CQApiClient client, CQEvent eventObj);
        ///
        public delegate Task<CQResponse> OnEventDelegateAsync (CQApiClient client, CQEvent eventObj);
        /// <summary></summary>
        public event OnEventDelegate OnEvent;
        /// <summary>
        /// 异步执行命令，忽略返回值
        /// </summary>
        public event OnEventDelegateAsync OnEventAsync;
        async Task ProcessMessageEvent (MessageEvent message) {
            switch (message) {
            case GroupMessageEvent group_message:
                var group_id = group_message.group_id;
                if (
                    group_table == null ||
                    !group_table[group_id].ContainsKey (SelfId)
                ) {
                    try {
                        var info = await SendRequestAsync (
                            new GetGroupMemberInfoRequest (
                                group_id,
                                SelfId,
                                no_cache: true
                            )
                        ) as GetGroupMemberInfoResult;
                        if (group_table != null)
                            group_table[group_id][SelfId] = info.memberInfo;
                        group_message.self_info = info.memberInfo;
                    } catch { }
                } else {
                    group_message.self_info
                        = group_table[group_id][SelfId];
                }
                break;
            case PrivateMessageEvent private_message:
                break;
            case DiscussMessageEvent discuss_message:
                break;
            }
        }
        /// <summary></summary>
        protected async Task<CQResponse> HandleEvent (CQEvent e) {
            Log.Debug ($"收到了完整的上报事件{e.postType}");
            switch (e) {
            case HeartbeatEvent heartbeat:
                if (heartbeat.status.online) {
                    Alive = true;
                    alive_counter++;
                    var task = Task.Run (() => {
                        System.Threading.Thread.Sleep (
                            (int) heartbeat.interval
                        );
                        if (alive_counter-- == 0)
                            Alive = false;
                    });
                } else Alive = false;
                return new EmptyResponse ();
            case LifecycleEvent lifecycle:
                Alive = lifecycle.enabled ? true : false;
                return new EmptyResponse ();
            case MessageEvent message:
                Alive = true;
                if (message_table != null)
                    message_table.Log (
                        message.message_id,
                        message.message
                    );
                await ProcessMessageEvent (message);
                if (DialoguePool.Handle (this, (e as MessageEvent)))
                    return new EmptyResponse ();
                break;
            }
            try {
                if (OnEventAsync != null)
                    await OnEventAsync (this, e);
                if (OnEvent != null)
                    return OnEvent (this, e);
                else
                    return new EmptyResponse ();
            } catch (InvokeDialogueException d) {
                DialoguePool.Join ((e as MessageEvent).GetEndpoint (), d.content);
                return new EmptyResponse ();
            }
        }
    }
}