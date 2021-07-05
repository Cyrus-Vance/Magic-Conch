using System;
using System.Collections.Generic;
using System.Text;

namespace MagicConchQQRobot.Modules.Utils
{
    public static class FileSizeHelper
    {

        /// <summary>
        /// 计算文件大小函数(保留两位小数),Size为字节大小
        /// </summary>
        /// <param name="size">初始文件大小</param>
        /// <returns></returns>
        public static string GetFileSize(long size)
        {
            var num = 1024.00; //byte


            if (size < num)
                return size + "B";
            if (size < Math.Pow(num, 2))
                return (size / num).ToString("f2") + "KB"; //kb
            if (size < Math.Pow(num, 3))
                return (size / Math.Pow(num, 2)).ToString("f2") + "MB"; //M
            if (size < Math.Pow(num, 4))
                return (size / Math.Pow(num, 3)).ToString("f2") + "GB"; //G


            return (size / Math.Pow(num, 4)).ToString("f2") + "TB"; //T
        }
    }
}
