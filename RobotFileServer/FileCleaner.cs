using RobotFileServer.Modules.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace RobotFileServer
{
    public static class FileCleaner
    {
        private static readonly Timer cleanTimer = new Timer();
        public static Dictionary<string, long> KillingFiles = new Dictionary<string, long>();

        public static void AddCleanFile(string filePath, int seconds)
        {
            KillingFiles.Add(filePath, TimestampHelper.GetTimestampSecond() + seconds);
        }

        public static void InitTimer()
        {
            cleanTimer.Interval = 1000;
            cleanTimer.Elapsed += Timer_Elapsed;
            cleanTimer.Enabled = true;

        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var item in KillingFiles)
            {
                if (TimestampHelper.GetTimestampSecond()>item.Value )
                {
                    try
                    {
                        File.Delete(item.Key);
                        Console.WriteLine($"文件{item.Key}删除成功");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"文件{item.Key}删除失败:" + ex.Message);
                    }
                    KillingFiles.Remove(item.Key);
                }
            }
        }
    }
}
