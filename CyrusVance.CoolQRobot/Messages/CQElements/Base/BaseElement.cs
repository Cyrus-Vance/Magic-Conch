using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyrusVance.CoolQRobot.Messages.CQElements.Base {
    /// <summary>
    /// 消息元素，即cqhttp所定义的消息段
    /// </summary>
    /// <remarks>
    /// should NEVER be constructed or used directly
    /// </remarks>
    [JsonObject (MemberSerialization.OptIn)]
    public class Element {
        /// <summary>
        /// 消息段类型
        /// </summary>
        [JsonProperty]
        public string type { get; private set; }
        /// <summary>
        /// represents the true message 
        /// </summary>
        [JsonProperty]
        public virtual Dictionary<string, string> data { get; private set; }
        /// <summary>
        /// 表示消息段是否只能单独发送
        /// </summary>
        public bool isSingle = false;

        /// <summary></summary>
        public static Message operator + (Element a, Element b) {
            return new Message (a, b);
        }
        /// <summary></summary>
        public static bool operator == (Element a, Element b) {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            if (a.type != b.type) return false;
            var i = a.data.GetEnumerator ();
            var j = b.data.GetEnumerator ();
            while (i.Current.Key == j.Current.Key && i.Current.Value == j.Current.Value) {
                if (i.MoveNext ()) {
                    if (j.MoveNext ()) continue;
                    else break;
                } else {
                    j.MoveNext ();
                    break;
                }
            }
            if (!(i.Current.Key is null) || !(j.Current.Key is null)) return false;
            return true;
        }
        /// <summary></summary>
        public static bool operator != (Element a, Element b) {
            return !(a == b);
        }
        /// <summary></summary>
        public override bool Equals (object b) {
            if (b is Element) {
                return this == (b as Element);
            }
            return false;
        }
        /// <summary></summary>
        public override int GetHashCode () {
            int hash = 1;
            foreach (var i in data)
                hash ^= i.GetHashCode ();
            return hash;
        }
        /// <summary>
        /// 将消息段序列化为CQ码格式，即酷Q原生消息格式
        /// <see>https://d.cqp.me/Pro/CQ码</see>
        /// </summary>
        /// <value>CQCode</value>
        public override string ToString () {
            if (type == "text")
                return Encoder.EncodeText (data["text"]);
            string paramBuilder = "";
            foreach (var i in data)
                if (i.Value.Length > 0)
                    paramBuilder += $",{i.Key}={Encoder.EncodeValue(i.Value)}";
            return string.Format (
                "[CQ:{0}{1}]",
                type, paramBuilder.TrimEnd (' ')
            );
        }
        /// <summary>
        /// 手动构造一个消息段，一般用不到
        /// </summary>
        /// <param name="type">消息段类型</param>
        /// <param name="dict">手动输入的键值对</param>
        public Element (string type, params (string key, string value) [] dict) {
            this.type = type;
            data = new Dictionary<string, string> ();
            foreach (var i in dict)
                data.Add (i.key, i.value);
        }
        /// <summary>
        /// 构造消息段，一般不会手动调用
        /// </summary>
        /// <param name="type">消息段类型</param>
        /// <param name="dict">消息段键值对</param>
        public Element (string type, Dictionary<string, string> dict) {
            this.type = type;
            data = dict;
        }
        /// <summary>
        /// 构造一个类型为type的消息段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static Element GetElement (string type, Dictionary<string, string> dict) {
            try {
                switch (type) {
                case "text":
                    return new ElementText (dict["text"]);
                case "image":
                    return new ElementImage (dict["url"]);
                case "at":
                    return new ElementAt (long.Parse (dict["qq"]));
                case "record":
                    return new ElementRecord (dict["file"]);
                case "face":
                    return new ElementFace (dict["id"]);
                case "bface":
                    return new ElementFace (dict["id"], "bface");
                case "sface":
                    return new ElementFace (dict["id"], "sface");
                case "emoji":
                    return new ElementFace (dict["id"], "emoji");
                case "shake":
                    return new ElementShake ();
                case "share":
                    string content, image;
                    if (dict.ContainsKey ("content"))
                        content = dict["content"];
                    else content = "";
                    if (dict.ContainsKey ("image"))
                        image = dict["image"];
                    else image = "";
                    return new ElementShare (
                        dict["url"], dict["title"],
                        content, image
                    );
                case "location":
                    return new ElementLocation (
                        float.Parse (dict["lat"]),
                        float.Parse (dict["lon"]),
                        dict["title"],
                        dict["content"],
                        dict["style"]
                    );
                case "contact":
                    return new ElementContact (
                        dict["type"] == "private" ?
                        Enums.MessageType.private_ :
                        dict["type"] == "group" ?
                        Enums.MessageType.group_ :
                        Enums.MessageType.discuss_,
                        long.Parse (dict["id"])
                    );
                default:
                    Log.Warn ($"未能解析type为{type}的元素");
                    return new Element (type, dict);
                }
            } catch (KeyNotFoundException) {
                throw new Exceptions.ErrorElementException ($"type为{type}的元素反序列化过程中缺少必要的参数");
            }
        }
    }
}