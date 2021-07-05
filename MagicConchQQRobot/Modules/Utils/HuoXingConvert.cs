using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;

namespace MagicConchQQRobot.Modules.Utils
{
    class HuoXingConvert
    {
        /// <summary>
        /// 火星文网站Url
        /// </summary>
        private const string HuoxingProviderUrl = "http://www.fzlft.com/huo/?tdsourcetag=";

        /// <summary>
        /// 将字符串转换为火星文形式
        /// </summary>
        /// <param name="inputText">要输入的文本</param>
        /// <returns></returns>
        public static string ConvertToHuoXing(string inputText)
        {
            int retryCount = 0;
            while (true)
            {
                string htmlText = HttpHelper.HttpPost(HuoxingProviderUrl, "t=&q=" + HttpUtility.UrlEncode(inputText));
                HtmlDocument historyDoc = new HtmlDocument();
                historyDoc.LoadHtml(htmlText);
                var headerCollection = historyDoc.DocumentNode.SelectNodes("//textarea[@id='result']");
                if (headerCollection != null)
                {
                    string[] returnTextList = headerCollection[0].InnerText
                        .Split("\r\n\r\n------------------------------------\r\n");
                    return HttpUtility.HtmlDecode(returnTextList[new Random().Next(returnTextList.Length)]);
                }
                else
                {
                    Console.WriteLine("火星文网站出现问题，正在重试中……");
                    retryCount++;
                    Thread.Sleep(2500);
                }
                if (retryCount == 3) return inputText;
            }
        }
    }
}
