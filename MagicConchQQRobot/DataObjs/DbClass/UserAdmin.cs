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
    [BindIndex("PRIMARY", true, "uid")]
    [BindTable("User_Admin", Description = "", ConnName = "CvqRobotDB", DbType = DatabaseType.None)]
    public partial class UserAdmin
    {
        #region 属性
        private Int32 _Uid;
        /// <summary>管理员QQ</summary>
        [DisplayName("管理员QQ")]
        [Description("管理员QQ")]
        [DataObjectField(true, false, false, 0)]
        [BindColumn("Uid", "管理员QQ", "int(10) unsigned")]
        public Int32 Uid { get => _Uid; set { if (OnPropertyChanging("Uid", value)) { _Uid = value; OnPropertyChanged("Uid"); } } }

        private Int32 _AdminLevel;
        /// <summary>管理员等级</summary>
        [DisplayName("管理员等级")]
        [Description("管理员等级")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("admin_level", "管理员等级", "int(10) unsigned")]
        public Int32 AdminLevel { get => _AdminLevel; set { if (OnPropertyChanging("AdminLevel", value)) { _AdminLevel = value; OnPropertyChanged("AdminLevel"); } } }
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
                    case "Uid": return _Uid;
                    case "AdminLevel": return _AdminLevel;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "Uid": _Uid = value.ToInt(); break;
                    case "AdminLevel": _AdminLevel = value.ToInt(); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得UserAdmin字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>管理员QQ</summary>
            public static readonly Field Uid = FindByName("Uid");

            /// <summary>管理员等级</summary>
            public static readonly Field AdminLevel = FindByName("AdminLevel");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得UserAdmin字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>管理员QQ</summary>
            public const String Uid = "Uid";

            /// <summary>管理员等级</summary>
            public const String AdminLevel = "AdminLevel";
        }
        #endregion
    }
}