using MagicConchQQRobot.Globals;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace MagicConchQQRobot.Modules.Utils
{
    class HttpHelper
    {
        public const string PixivRefererUrl = "https://www.pixiv.net/";
        /// <summary>
        /// POST方法
        /// </summary>
        /// <param name="Url">要访问的地址</param>
        /// <param name="postDataStr">PostBody的内容，例如a=1&amp;b=2</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //request.Referer = GlobalSet.Url_Bilibili;
            request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookieContainer ?? GlobalObj.GlobalCookies;
            Encoding encoding = Encoding.UTF8;
            byte[] postData = encoding.GetBytes(postDataStr);
            request.ContentLength = postData.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postData, 0, postData.Length);
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //GET方法
        public static string HttpGet(string Url, string postDataStr = "", CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (string.IsNullOrEmpty(postDataStr) ? "" : "?") + postDataStr);
            request.Method = "GET";
            //request.Referer = GlobalSet.Url_Bilibili;
            request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = cookieContainer ?? GlobalObj.GlobalCookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static string DownloadTempFile(string url)
        {
            string fileTempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(fileTempPath)) Directory.CreateDirectory(fileTempPath);
            string savePath = Path.Combine(fileTempPath, Guid.NewGuid().ToString());
            DownloadFile(url, savePath);
            return savePath;
        }

        public static void DownloadFile(string url, string savePath, string referer = null, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            if (referer != null) request.Referer = referer;
            request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = cookieContainer ?? GlobalObj.GlobalCookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            Stream fileSaveStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] bArr = new byte[1024];
            int size = myResponseStream.Read(bArr, 0, bArr.Length);
            long bytescount = 0;
            try
            {
                while (size > 0)
                {
                    bytescount += size;
                    fileSaveStream.Write(bArr, 0, size);
                    size = myResponseStream.Read(bArr, 0, bArr.Length);
                    //if (size != 1024)
                    //{
                    //    FrmSaver.LogHelper.AddLog($"size为{size}");
                    //}
                    //Debug.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]已写入1024字节,当前为第{bytescount}KB");
                }
            }
            catch
            {
                Console.WriteLine($"图片下载发生错误！！！");
            }
            fileSaveStream.Flush();
            fileSaveStream.Close();


            response.Close();
            // Release the response object resources.
            myResponseStream.Close();
        }
    }
}
