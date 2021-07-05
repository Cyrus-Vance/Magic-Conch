using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace MagicConchQQRobot.DataObjs.DbClass
{
    /// <summary></summary>
    [Serializable]
    [DataObject]
    [BindTable("Member", Description = "", ConnName = "SuperNoobDB", DbType = DatabaseType.None)]
    public partial class Member
    {
        #region 属性
        private Int32 _id;
        /// <summary></summary>
        [DisplayName("id")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("id", "", "int(11)")]
        public Int32 id { get => _id; set { if (OnPropertyChanging("id", value)) { _id = value; OnPropertyChanged("id"); } } }

        private Int64 _qq;
        /// <summary></summary>
        [DisplayName("qq")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("qq", "", "bigint(20)")]
        public Int64 qq { get => _qq; set { if (OnPropertyChanging("qq", value)) { _qq = value; OnPropertyChanged("qq"); } } }

        private String _Code;
        /// <summary></summary>
        [DisplayName("Code")]
        [DataObjectField(false, false, false, 255)]
        [BindColumn("Code", "", "varchar(255)")]
        public String Code { get => _Code; set { if (OnPropertyChanging("Code", value)) { _Code = value; OnPropertyChanged("Code"); } } }

        private Int64 _JoinTime;
        /// <summary></summary>
        [DisplayName("JoinTime")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("join_time", "", "bigint(20)")]
        public Int64 JoinTime { get => _JoinTime; set { if (OnPropertyChanging("JoinTime", value)) { _JoinTime = value; OnPropertyChanged("JoinTime"); } } }

        private Int32 _Status;
        /// <summary></summary>
        [DisplayName("Status")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Status", "", "int(11)")]
        public Int32 Status { get => _Status; set { if (OnPropertyChanging("Status", value)) { _Status = value; OnPropertyChanged("Status"); } } }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case "id": return _id;
                    case "qq": return _qq;
                    case "Code": return _Code;
                    case "JoinTime": return _JoinTime;
                    case "Status": return _Status;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "id": _id = value.ToInt(); break;
                    case "qq": _qq = value.ToLong(); break;
                    case "Code": _Code = Convert.ToString(value); break;
                    case "JoinTime": _JoinTime = value.ToLong(); break;
                    case "Status": _Status = value.ToInt(); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得Member字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary></summary>
            public static readonly Field id = FindByName("id");

            /// <summary></summary>
            public static readonly Field qq = FindByName("qq");

            /// <summary></summary>
            public static readonly Field Code = FindByName("Code");

            /// <summary></summary>
            public static readonly Field JoinTime = FindByName("JoinTime");

            /// <summary></summary>
            public static readonly Field Status = FindByName("Status");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得Member字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary></summary>
            public const String id = "id";

            /// <summary></summary>
            public const String qq = "qq";

            /// <summary></summary>
            public const String Code = "Code";

            /// <summary></summary>
            public const String JoinTime = "JoinTime";

            /// <summary></summary>
            public const String Status = "Status";
        }
        #endregion
    }
}