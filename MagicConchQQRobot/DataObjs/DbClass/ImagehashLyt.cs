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
    [BindIndex("PRIMARY", true, "hash")]
    [BindTable("ImageHash_LYT", Description = "", ConnName = "CvqRobotDB", DbType = DatabaseType.None)]
    public partial class ImagehashLyt
    {
        #region 属性
        private String _Hash;
        /// <summary>哈希值</summary>
        [DisplayName("哈希值")]
        [Description("哈希值")]
        [DataObjectField(true, false, false, 255)]
        [BindColumn("Hash", "哈希值", "varchar(255)")]
        public String Hash { get => _Hash; set { if (OnPropertyChanging("Hash", value)) { _Hash = value; OnPropertyChanged("Hash"); } } }

        private String _Filename;
        /// <summary>文件名</summary>
        [DisplayName("文件名")]
        [Description("文件名")]
        [DataObjectField(false, false, false, 255)]
        [BindColumn("Filename", "文件名", "varchar(255)")]
        public String Filename { get => _Filename; set { if (OnPropertyChanging("Filename", value)) { _Filename = value; OnPropertyChanged("Filename"); } } }

        private Int32 _HashType;
        /// <summary>哈希类型（1：SHA-1）</summary>
        [DisplayName("哈希类型")]
        [Description("哈希类型（1：SHA-1）")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("hash_type", "哈希类型（1：SHA-1）", "int(11)")]
        public Int32 HashType { get => _HashType; set { if (OnPropertyChanging("HashType", value)) { _HashType = value; OnPropertyChanged("HashType"); } } }

        private Int64 _UploadTime;
        /// <summary>上传时间</summary>
        [DisplayName("上传时间")]
        [Description("上传时间")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("upload_time", "上传时间", "bigint(20) unsigned")]
        public Int64 UploadTime { get => _UploadTime; set { if (OnPropertyChanging("UploadTime", value)) { _UploadTime = value; OnPropertyChanged("UploadTime"); } } }

        private Int64 _Uid;
        /// <summary>上传者的QQ</summary>
        [DisplayName("上传者的QQ")]
        [Description("上传者的QQ")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Uid", "上传者的QQ", "bigint(20)")]
        public Int64 Uid { get => _Uid; set { if (OnPropertyChanging("Uid", value)) { _Uid = value; OnPropertyChanged("Uid"); } } }
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
                    case "Hash": return _Hash;
                    case "Filename": return _Filename;
                    case "HashType": return _HashType;
                    case "UploadTime": return _UploadTime;
                    case "Uid": return _Uid;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "Hash": _Hash = Convert.ToString(value); break;
                    case "Filename": _Filename = Convert.ToString(value); break;
                    case "HashType": _HashType = value.ToInt(); break;
                    case "UploadTime": _UploadTime = value.ToLong(); break;
                    case "Uid": _Uid = value.ToLong(); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得ImagehashLyt字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>哈希值</summary>
            public static readonly Field Hash = FindByName("Hash");

            /// <summary>文件名</summary>
            public static readonly Field Filename = FindByName("Filename");

            /// <summary>哈希类型（1：SHA-1）</summary>
            public static readonly Field HashType = FindByName("HashType");

            /// <summary>上传时间</summary>
            public static readonly Field UploadTime = FindByName("UploadTime");

            /// <summary>上传者的QQ</summary>
            public static readonly Field Uid = FindByName("Uid");

            static Field FindByName(String name) => Meta.Table.FindByName(name);
        }

        /// <summary>取得ImagehashLyt字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>哈希值</summary>
            public const String Hash = "Hash";

            /// <summary>文件名</summary>
            public const String Filename = "Filename";

            /// <summary>哈希类型（1：SHA-1）</summary>
            public const String HashType = "HashType";

            /// <summary>上传时间</summary>
            public const String UploadTime = "UploadTime";

            /// <summary>上传者的QQ</summary>
            public const String Uid = "Uid";
        }
        #endregion
    }
}