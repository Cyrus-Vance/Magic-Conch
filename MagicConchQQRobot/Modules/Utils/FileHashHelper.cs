using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MagicConchQQRobot.Modules.Utils
{
    static class FileHashHelper
    {
        public static string CalcFileHash(string filePath)
        {
            var hash = SHA1.Create();
            var stream = new FileStream(filePath, FileMode.Open);
            byte[] hashByte = hash.ComputeHash(stream);
            stream.Close();
            //将字节数组转换成十六进制的字符串形式
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hashByte.Length; i++)
            {
                stringBuilder.Append(hashByte[i].ToString("x2"));
            }
#if DEBUG
            Console.WriteLine($"{filePath}的Hash值为:{stringBuilder}");
#endif
            return stringBuilder.ToString();
        }
    }
}
