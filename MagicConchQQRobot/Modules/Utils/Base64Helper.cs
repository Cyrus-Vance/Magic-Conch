using System;
using System.Collections.Generic;
using System.Text;

namespace MagicConchQQRobot.Modules.Utils
{
    class Base64Helper
    {
        /// <summary>
        /// 字符串转base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            //转成 Base64 形式的 System.String  
            str = Convert.ToBase64String(b);
            return str;

        }

        /// <summary>
        /// base64还原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            byte[] c = Convert.FromBase64String(str);
            str = Encoding.Default.GetString(c);
            return str;
        }
    }
}
