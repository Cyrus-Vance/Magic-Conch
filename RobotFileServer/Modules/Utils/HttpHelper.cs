using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobotFileServer.Modules.Utils
{
    public static class HttpHelper
    {
        /// <summary>
        /// POST方法
        /// </summary>
        /// <param name="Url">要访问的地址</param>
        /// <param name="postDataStr">PostBody的内容，例如a=1&amp;b=2</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //request.Referer = GlobalSet.Url_Bilibili;
            //request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "application/x-www-form-urlencoded";
           // request.CookieContainer = GlobalObj.GlobalCookies;
            Encoding encoding = Encoding.UTF8;
            byte[] postData = encoding.GetBytes(postDataStr);
            request.ContentLength = postData.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postData, 0, postData.Length);
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //GET方法
        public static string HttpGet(string Url, string postDataStr = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (string.IsNullOrEmpty(postDataStr) ? "" : "?") + postDataStr);
            request.Method = "GET";
            //request.Referer = GlobalSet.Url_Bilibili;
            //request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "text/html;charset=UTF-8";
            //request.CookieContainer = GlobalObj.GlobalCookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static void DownloadFile(string url, string savePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.Referer = GlobalSet.Url_Bilibili;
            //request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "text/html;charset=UTF-8";
            //request.CookieContainer = GlobalObj.GlobalCookies;
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
