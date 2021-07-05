using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.ApiCall.Results;
using CyrusVance.CoolQRobot.Utils;
using System.Threading.Tasks;

namespace CyrusVance.CoolQRobot.Clients
{
    /// <summary></summary>
    public partial class CQApiClient
    {
        /// <summary>
        /// 当前酷Q实例的各种参数
        /// </summary>
        public InstanceVersionInfo instance_version_info = null;
        /// <summary>
        /// 当前实例的QQ号
        /// </summary>
        public long SelfId { get; private set; }
        /// <summary>
        /// 当前实例的QQ昵称
        /// </summary>
        public string SelfNick { get; private set; }
        /// <summary>
        /// 连接到的实例是否为酷Q pro
        /// </summary>
        public bool IsPro
        {
            get
            {
                return instance_version_info.coolq_edition == "pro";
            }
        }
        /// <summary>
        /// 表示插件是否正常运行
        /// </summary>
        public bool Alive { get; private set; }
        /// <summary>
        /// 是否已经初始化完成（检查连通性并获取self_id与self_nick）
        /// </summary>
        public bool Initiated
        {
            get
            {
                return initiate_task.IsCompleted;
            }
        }
        int alive_counter = 0;
        /// <summary>
        /// 指向本实例的群组记录对象
        /// </summary>
        public GroupTable group_table = null;
        /// <summary>
        /// 消息记录
        /// </summary>
        public MessageTable message_table = null;

        ///
        protected Callers.ICaller caller;
        ///
        protected Listeners.IListener listener;

        /// <summary></summary>
        public CQApiClient(
            Callers.ICaller caller,
            Listeners.IListener listener,
            bool use_group_table = false,
            bool use_message_table = false
        )
        {
            this.caller = caller;
            this.listener = listener;
            if (use_group_table) group_table = new GroupTable();
            if (use_message_table) message_table = new MessageTable();
        }
        /// <summary>
        /// 
        /// </summary>
        protected Task initiate_task;
        /// <summary>
        /// 检查连通性并获取self_id与self_nick
        /// </summary>
        protected async Task Initiate()
        {
            GetLoginInfoResult loginInfo =
                await SendRequestAsync(new GetLoginInfoRequest())
            as GetLoginInfoResult;
            GetVersionInfoResult versionInfo =
                await SendRequestAsync(new GetVersionInfoRequest())
            as GetVersionInfoResult;
            SelfId = loginInfo.user_id;
            SelfNick = loginInfo.nickname;
            instance_version_info = versionInfo.instanceVersionInfo;
        }
    }
}