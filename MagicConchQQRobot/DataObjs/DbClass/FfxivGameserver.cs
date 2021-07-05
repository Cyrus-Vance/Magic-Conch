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
    [BindTable("FFXIV_GameServer", Description = "", ConnName = "CvqRobotDB", DbType = DatabaseType.None)]
    public partial class FfxivGameserver
    {
        #region 属性
        private Int32 _id;
        /// <summary></summary>
        [DisplayName("id")]
        [DataObjectField(true, true, false, 0)]
        [BindColumn("id", "", "int(11) unsigned")]
        public Int32 id { get => _id; set { if (OnPropertyChanging("id", value)) { _id = value; OnPropertyChanged("id"); } } }

        private String _ServerName;
        /// <summary></summary>
        [DisplayName("ServerName")]
        [DataObjectField(false, false, true, 255)]
        [BindColumn("server_name", "", "varchar(255)")]
        public String ServerName { get => _ServerName; set { if (OnPropertyChanging("ServerName", value)) { _ServerName = value; OnPropertyChanged("ServerName"); } } }

        private Int32 _AreaID;
        /// <summary></summary>
        [DisplayName("AreaID")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("area_id", "", "int(11)")]
        public Int32 AreaID { get => _AreaID; set { if (OnPropertyChanging("AreaID", value)) { _AreaID = value; OnPropertyChanged("AreaID"); } } }

        private Int32 _GroupID;
        /// <summary></summary>
        [DisplayName("GroupID")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("group_id", "", "int(11)")]
        public Int32 GroupID { get => _GroupID; set { if (OnPropertyChanging("GroupID", value)) { _GroupID = value; OnPropertyChanged("GroupID"); } } }
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
                    case "ServerName": return _ServerName;
                    case "AreaID": return _AreaID;
                    case "GroupID": return _GroupID;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "id": _id = value.ToInt(); break;
                    case "ServerName": _ServerName = Convert.ToString(value); break;
                    case "AreaID": _AreaID = value.ToInt(); break;
                    case "GroupID": _GroupID = value.ToInt(); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得FfxivGameserver字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary></summary>
            public static readonly Field id = FindByName("id");

            /// <summary></summary>
            public static readonly Field ServerName = FindByName("ServerName");

            /// <summary></summary>
            public static readonly Field AreaID = FindByName("AreaID");

            /// <summary></summary>
            public static readonly Field GroupID = FindByName("GroupID");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得FfxivGameserver字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary></summary>
            public const String id = "id";

            /// <summary></summary>
            public const String ServerName = "ServerName";

            /// <summary></summary>
            public const String AreaID = "AreaID";

            /// <summary></summary>
            public const String GroupID = "GroupID";
        }
        #endregion
    }
}