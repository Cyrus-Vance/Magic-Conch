using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MagicConchQQRobot.Modules.Utils
{
    class TimeDelay
    {
        public static void RandomPause()
        {
            Random rnd = new Random();
            int delayTime = rnd.Next(380, 800);
#if DEBUG
            Console.WriteLine($"[随机延时]随机反应时间为：{delayTime}");
#endif
            Thread.Sleep(delayTime);
        }
    }
}
