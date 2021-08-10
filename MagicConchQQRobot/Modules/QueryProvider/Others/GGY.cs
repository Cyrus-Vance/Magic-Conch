using HtmlAgilityPack;
using MagicConchQQRobot.Modules.SecretProvider;
using MagicConchQQRobot.Modules.Utils;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace MagicConchQQRobot.Modules.QueryProvider.Others
{
    class GGY
    {
        public const string GGYLoginUrl = "https://www.ggy.net/login";
        public const string GGYUserPanelUrl = "https://www.ggy.net/clientarea.php";
        public const string GGYNetworkStateUrl = "https://www.ggy.net/clientarea.php?action=productdetails&id=12067";
        public static CookieContainer cookieContainer = new();

        public class MachineStateReturn
        {
            public string UsedData { get; set; }
            public string TotalData { get; set; }
            public string RemainData { get; set; }
        }

        public static bool Login()
        {

            Console.WriteLine("正在登录[拜登 - HK节点]后台管理……");
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"GGYIdentity.dat")))
            {
                try
                {
                    using Stream stream = File.Open(AppDomain.CurrentDomain.BaseDirectory + @"GGYIdentity.dat", FileMode.Open);
                    Console.WriteLine("正在读取本地保存的[拜登 - HK节点]身份信息... ");
                    BinaryFormatter formatter = new();
                    cookieContainer = (CookieContainer)formatter.Deserialize(stream);
                    stream.Close();
                    Console.WriteLine("[拜登 - HK节点]身份信息读取完毕，正在检测身份可用性……");

                    MachineStateReturn machineStateReturn = GetMachineNetworkState();
                    Console.WriteLine($"当前身份可用，[拜登 - HK节点]当前使用流量状况:{machineStateReturn.UsedData}/{machineStateReturn.TotalData}，剩余流量：{machineStateReturn.RemainData}");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("读取本地身份信息失败: " + e.ToString());
                    Console.WriteLine("正在尝试重新登录……");
                }
            }

            string loginHtmlText = HttpHelper.HttpGet(GGYLoginUrl, cookieContainer: cookieContainer);

            //读取csrftoken
            HtmlDocument loginDoc = new();
            loginDoc.LoadHtml(loginHtmlText);
            var tokenNode = loginDoc.DocumentNode.SelectSingleNode("//input[@name='token']");
            string tokenString = tokenNode.GetAttributeValue("value", "");

            Console.WriteLine("Token获取完成，正在登录[拜登 - HK节点]中...");

            HttpHelper.HttpPost(GGYLoginUrl, $"username={SecretData.GetSectionData("GGYVPN:Username")}&password={SecretData.GetSectionData("GGYVPN:Password")}&token={tokenString}", cookieContainer: cookieContainer);

            string loginUserPanelText = HttpHelper.HttpGet(GGYUserPanelUrl, cookieContainer: cookieContainer);

            HtmlDocument afterLoginDoc = new();
            afterLoginDoc.LoadHtml(loginUserPanelText);
            var nameNode = afterLoginDoc.DocumentNode.SelectSingleNode("//div[@class='info-name']");

            if (nameNode != null)
            {
                Console.WriteLine($"[拜登 - HK节点]登录成功!名称为{nameNode.InnerText}");
                using Stream stream = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"GGYIdentity.dat"));
                try
                {
                    Console.WriteLine("正在保存[拜登 - HK节点]登录信息... ");
                    BinaryFormatter formatter = new();
                    formatter.Serialize(stream, cookieContainer);
                    stream.Close();
                    Console.WriteLine("[拜登 - HK节点]登录信息保存完毕.");

                    MachineStateReturn machineStateReturn = GetMachineNetworkState();
                    Console.WriteLine($"当前身份可用，[拜登 - HK节点]当前使用流量状况:{machineStateReturn.UsedData}/{machineStateReturn.TotalData}，剩余流量：{machineStateReturn.RemainData}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("登录信息保存失败: " + ex.GetType());
                }
            }
            else
            {
                Console.WriteLine("[拜登 - HK节点]登录失败，请联系开发者获取详情!");
            }
            return false;
        }

        public static MachineStateReturn GetMachineNetworkState()
        {
            string userPanelText = HttpHelper.HttpGet(GGYNetworkStateUrl, cookieContainer: cookieContainer);

            HtmlDocument panelDoc = new();
            panelDoc.LoadHtml(userPanelText);

            var targetNode = panelDoc.DocumentNode.SelectNodes("//div[@class='col-md-4']")[0].SelectSingleNode("./div/div");

            var usedData = targetNode.SelectSingleNode("./span").InnerText.Trim();
            var deletedUsedDataNode = targetNode;

            deletedUsedDataNode.SelectSingleNode("./span").Remove();

            var totalData = deletedUsedDataNode.InnerText.Trim().Substring(2);

            MachineStateReturn machineStateReturn = new()
            {
                UsedData = usedData,
                TotalData = totalData,
                RemainData = (totalData.Replace("GB", "").ToDouble() - usedData.Replace("GB", "").ToDouble()).ToString() + "GB"
            };
            return machineStateReturn;
        }
    }
}
