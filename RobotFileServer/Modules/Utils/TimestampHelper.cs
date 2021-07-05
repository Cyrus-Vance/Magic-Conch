using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotFileServer.Modules.Utils
{
    public class TimestampHelper
    {
        /// <summary>
        /// 获取秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampSecond()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取毫秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampMilliSecond()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}
