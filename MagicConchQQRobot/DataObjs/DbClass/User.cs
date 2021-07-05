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
    [BindTable("User", Description = "", ConnName = "CvqRobotDB", DbType = DatabaseType.None)]
    public partial class User
    {
        #region 属性
        private Int64 _Uid;
        /// <summary>用户QQ号</summary>
        [DisplayName("用户QQ号")]
        [Description("用户QQ号")]
        [DataObjectField(true, false, false, 0)]
        [BindColumn("Uid", "用户QQ号", "bigint(255) unsigned")]
        public Int64 Uid { get => _Uid; set { if (OnPropertyChanging("Uid", value)) { _Uid = value; OnPropertyChanged("Uid"); } } }

        private Int32 _Level;
        /// <summary>用户等级</summary>
        [DisplayName("用户等级")]
        [Description("用户等级")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Level", "用户等级", "int(11) unsigned")]
        public Int32 Level { get => _Level; set { if (OnPropertyChanging("Level", value)) { _Level = value; OnPropertyChanged("Level"); } } }

        private String _Nickname;
        /// <summary>SuperNoob队员的代号</summary>
        [DisplayName("SuperNoob队员的代号")]
        [Description("SuperNoob队员的代号")]
        [DataObjectField(false, false, false, 255)]
        [BindColumn("Nickname", "SuperNoob队员的代号", "varchar(255)")]
        public String Nickname { get => _Nickname; set { if (OnPropertyChanging("Nickname", value)) { _Nickname = value; OnPropertyChanged("Nickname"); } } }

        private Int32 _Banned;
        /// <summary>用户是否被封禁</summary>
        [DisplayName("用户是否被封禁")]
        [Description("用户是否被封禁")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Banned", "用户是否被封禁", "int(11) unsigned")]
        public Int32 Banned { get => _Banned; set { if (OnPropertyChanging("Banned", value)) { _Banned = value; OnPropertyChanged("Banned"); } } }

        private Int32 _OffenceCount;
        /// <summary>用户骚扰次数</summary>
        [DisplayName("用户骚扰次数")]
        [Description("用户骚扰次数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("offence_count", "用户骚扰次数", "int(11)")]
        public Int32 OffenceCount { get => _OffenceCount; set { if (OnPropertyChanging("OffenceCount", value)) { _OffenceCount = value; OnPropertyChanged("OffenceCount"); } } }

        private Int64 _Regtime;
        /// <summary>注册时的时间戳</summary>
        [DisplayName("注册时的时间戳")]
        [Description("注册时的时间戳")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Regtime", "注册时的时间戳", "bigint(20) unsigned")]
        public Int64 Regtime { get => _Regtime; set { if (OnPropertyChanging("Regtime", value)) { _Regtime = value; OnPropertyChanged("Regtime"); } } }

        private Int32 _ImageIdCount;
        /// <summary>每日图像识别的次数</summary>
        [DisplayName("每日图像识别的次数")]
        [Description("每日图像识别的次数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("ImageIdCount", "每日图像识别的次数", "int(11) unsigned")]
        public Int32 ImageIdCount { get => _ImageIdCount; set { if (OnPropertyChanging("ImageIdCount", value)) { _ImageIdCount = value; OnPropertyChanged("ImageIdCount"); } } }

        private Int64 _LastImageIdTime;
        /// <summary>最后一次图像识别的日期</summary>
        [DisplayName("最后一次图像识别的日期")]
        [Description("最后一次图像识别的日期")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("LastImageIdTime", "最后一次图像识别的日期", "bigint(255) unsigned")]
        public Int64 LastImageIdTime { get => _LastImageIdTime; set { if (OnPropertyChanging("LastImageIdTime", value)) { _LastImageIdTime = value; OnPropertyChanged("LastImageIdTime"); } } }
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
                    case "Level": return _Level;
                    case "Nickname": return _Nickname;
                    case "Banned": return _Banned;
                    case "OffenceCount": return _OffenceCount;
                    case "Regtime": return _Regtime;
                    case "ImageIdCount": return _ImageIdCount;
                    case "LastImageIdTime": return _LastImageIdTime;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "Uid": _Uid = value.ToLong(); break;
                    case "Level": _Level = value.ToInt(); break;
                    case "Nickname": _Nickname = Convert.ToString(value); break;
                    case "Banned": _Banned = value.ToInt(); break;
                    case "OffenceCount": _OffenceCount = value.ToInt(); break;
                    case "Regtime": _Regtime = value.ToLong(); break;
                    case "ImageIdCount": _ImageIdCount = value.ToInt(); break;
                    case "LastImageIdTime": _LastImageIdTime = value.ToLong(); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得User字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>用户QQ号</summary>
            public static readonly Field Uid = FindByName("Uid");

            /// <summary>用户等级</summary>
            public static readonly Field Level = FindByName("Level");

            /// <summary>SuperNoob队员的代号</summary>
            public static readonly Field Nickname = FindByName("Nickname");

            /// <summary>用户是否被封禁</summary>
            public static readonly Field Banned = FindByName("Banned");

            /// <summary>用户骚扰次数</summary>
            public static readonly Field OffenceCount = FindByName("OffenceCount");

            /// <summary>注册时的时间戳</summary>
            public static readonly Field Regtime = FindByName("Regtime");

            /// <summary>每日图像识别的次数</summary>
            public static readonly Field ImageIdCount = FindByName("ImageIdCount");

            /// <summary>最后一次图像识别的日期</summary>
            public static readonly Field LastImageIdTime = FindByName("LastImageIdTime");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得User字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>用户QQ号</summary>
            public const String Uid = "Uid";

            /// <summary>用户等级</summary>
            public const String Level = "Level";

            /// <summary>SuperNoob队员的代号</summary>
            public const String Nickname = "Nickname";

            /// <summary>用户是否被封禁</summary>
            public const String Banned = "Banned";

            /// <summary>用户骚扰次数</summary>
            public const String OffenceCount = "OffenceCount";

            /// <summary>注册时的时间戳</summary>
            public const String Regtime = "Regtime";

            /// <summary>每日图像识别的次数</summary>
            public const String ImageIdCount = "ImageIdCount";

            /// <summary>最后一次图像识别的日期</summary>
            public const String LastImageIdTime = "LastImageIdTime";
        }
        #endregion
    }
}