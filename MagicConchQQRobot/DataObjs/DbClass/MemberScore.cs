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
    [BindIndex("PRIMARY", true, "id")]
    [BindTable("Member_Score", Description = "", ConnName = "SuperNoobDB", DbType = DatabaseType.None)]
    public partial class MemberScore
    {
        #region 属性
        private Int32 _id;
        /// <summary></summary>
        [DisplayName("id")]
        [DataObjectField(true, false, false, 0)]
        [BindColumn("id", "", "int(11)")]
        public Int32 id { get => _id; set { if (OnPropertyChanging("id", value)) { _id = value; OnPropertyChanged("id"); } } }

        private Single _Bait;
        /// <summary></summary>
        [DisplayName("Bait")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("Bait", "", "float(11,2)")]
        public Single Bait { get => _Bait; set { if (OnPropertyChanging("Bait", value)) { _Bait = value; OnPropertyChanged("Bait"); } } }
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
                    case "Bait": return _Bait;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "id": _id = value.ToInt(); break;
                    case "Bait": _Bait = Convert.ToSingle(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得MemberScore字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary></summary>
            public static readonly Field id = FindByName("id");

            /// <summary></summary>
            public static readonly Field Bait = FindByName("Bait");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得MemberScore字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary></summary>
            public const String id = "id";

            /// <summary></summary>
            public const String Bait = "Bait";
        }
        #endregion
    }
}