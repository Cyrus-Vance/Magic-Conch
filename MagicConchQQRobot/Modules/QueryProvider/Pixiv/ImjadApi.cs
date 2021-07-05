using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.Utils;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Messages;
using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;
using MetadataExtractor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebPWrapper.Decoder;
using Directory = System.IO.Directory;

namespace MagicConchQQRobot.Modules.QueryProvider.Pixiv
{
    class ImjadApi
    {
        private const string ImjadUrl_v2 = "https://api.imjad.cn/pixiv/v2/";

        public static async Task GetImageDetailAsync(long groupId, long userId, string imageId, int selectedIndex = 0, bool isOriginalImageNeeded = false)
        {
            JObject jo = (JObject)JsonConvert.DeserializeObject(HttpHelper.HttpGet(ImjadUrl_v2 + $"?type=illust&id={imageId}"));
            int imageCount = jo["illust"]["meta_pages"].Count();
            if (selectedIndex >= 0 && selectedIndex <= imageCount)
            {
                bool xRestrict = jo["illust"]["x_restrict"].ToBoolean();

                if (xRestrict && !IdentityHelper.IsAdmin(userId) && IdentityHelper.GetUserAccessLevel(userId) <= 1)
                {
                    await GlobalObj.WSRobotClient.SendMessageAsync(
                            MessageType.group_,
                            groupId, new Message(new ElementAt(userId), new ElementText("太色了太色了，使不得使不得o(>﹏<)o~~")));
                    return;
                }

                int pageCount = jo["illust"]["page_count"].ToInt();
                string title = jo["illust"]["title"].ToString();
                string createTime = jo["illust"]["create_date"].ToString();
                //DateTime dt = DateTime.ParseExact(createTime, "yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);

                string outputText = $"\n标题：{title}\n画师：{jo["illust"]["user"]["name"]}（{jo["illust"]["user"]["id"]}）\n创作于：{createTime}（已转换为北京时间）";

                if (pageCount != 1)
                {
                    outputText += $"\n已为您找到{imageId}的图片详情\n当前为第{selectedIndex + 1}页，该作品共有{pageCount}页";
                }

                if (xRestrict) outputText += "\n太色了太色了，请少来一点~";
                outputText += "\n根据默认设定，已为您展示较高画质图片";
                outputText += "\n※Pixiv找图功能由Cyrus API Hub驱动，图片来自Pixiv，版权归原作者所有";

                string originalPhotoDownloadUrl = "";

                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                string tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());

                string downloadUrl = string.Empty;
                if (imageCount > 1)
                {
                    downloadUrl = jo["illust"]["meta_pages"][selectedIndex]["image_urls"]["large"].ToString();
                }
                else
                {
                    downloadUrl = jo["illust"]["image_urls"]["large"].ToString();
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);
                request.Method = "GET";
                request.Referer = "https://www.pixiv.net";
                request.UserAgent = GlobalSet.UserAgent_Chrome;
                request.ContentType = "text/html;charset=UTF-8";
                request.CookieContainer = GlobalObj.GlobalCookies;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                long fileSize = response.ContentLength;
                if (isOriginalImageNeeded)
                {
                    Task downloadTask = new(() =>
                    {
                        string originalPhotoUrl;
                        if (pageCount == 1)
                        {
                            originalPhotoUrl = jo["illust"]["meta_single_page"]["original_image_url"].ToString();
                        }
                        else
                        {
                            originalPhotoUrl = jo["illust"]["meta_pages"][selectedIndex]["image_urls"]["original"].ToString();
                        }
                        //MessageHelper.SendGroupTextMessageWithAt(groupId, userId, $"正在为您下载原图中……\n由于网络状况的特殊性可能需要一段时间哦~请耐心等待！");
                        string jsonText = HttpHelper.HttpGet("http://123.57.175.205:5420/api/files/image-url?fileUrl=" + originalPhotoUrl);
                        JObject imageJo = (JObject)JsonConvert.DeserializeObject(jsonText);
                        originalPhotoDownloadUrl = "http://123.57.175.205:5420/api/files/image?fileName=" + imageJo["url"];
                        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(imageJo["expire"]));
                        string expireTimeStr = "";
                        if (ts.Hours > 0)
                        {
                            expireTimeStr = ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds + "秒";
                        }
                        if (ts.Hours == 0 && ts.Minutes > 0)
                        {
                            expireTimeStr = ts.Minutes.ToString() + "分钟" + ts.Seconds + "秒";
                        }
                        if (ts.Hours == 0 && ts.Minutes == 0)
                        {
                            expireTimeStr = ts.Seconds + "秒";
                        }
                        GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId, new Message(new ElementAt(userId), new ElementText($"\n原图已经下载完毕，有效期为{expireTimeStr}！打开即可下载：{originalPhotoDownloadUrl}"))).Wait();
                    });
                    downloadTask.Start();
                }
                Stream myResponseStream = response.GetResponseStream();
                Stream fileSaveStream = new FileStream(tmpFileName, FileMode.OpenOrCreate, FileAccess.Write);
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
                        //Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]已写入1024字节,{bytescount}/{fileSize}");
                    }
                }
                catch
                {
                    Console.WriteLine($"图片下载发生错误！！！");
                }
                fileSaveStream.Flush();
                fileSaveStream.Close();

                List<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(tmpFileName).ToList();

                List<Element> singlePicMsgList = new List<Element>();
                var fileTypeItem = directories.Find(e => e.Name.Equals("File Type", StringComparison.Ordinal));
                var tagList = fileTypeItem.Tags.ToList();
                if (tagList.Find(e => e.Name.Equals("Detected File Type Name", StringComparison.Ordinal)).Description.Equals("WebP", StringComparison.Ordinal))
                {
                    //WebPExecuteDownloader.Download();

                    var builder = new WebPDecoderBuilder();

                    var encoder = builder
                        //.Resize(32, 0) // 調整寬度為32，等比縮放(因為高度為0)
                        .Build(); // 建立解碼器
                    if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                    string outputTmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());

                    using (var outputFile = File.Open(outputTmpFileName, FileMode.Create))
                    using (var inputFile = File.Open(tmpFileName, FileMode.Open))
                    {
                        encoder.Decode(inputFile, outputFile); // 解碼
                    }
                    FileStream fs = new(outputTmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行读取
                    BinaryReader br = new(fs);
                    byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                    fs.Close();

                    singlePicMsgList.Add(new ElementImage(imgBytesIn));
                    File.Delete(outputTmpFileName);
                }
                else
                {
                    FileStream fs = new(tmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行读取
                    BinaryReader br = new(fs);
                    byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                    fs.Close();
                    singlePicMsgList.Add(new ElementImage(imgBytesIn));
                    File.Delete(tmpFileName);
                }


                singlePicMsgList.Add(new ElementText(outputText));
                if (isOriginalImageNeeded && originalPhotoDownloadUrl != null) singlePicMsgList.Add(new ElementText($"\n原图正在下载中，下载完毕以后会通知您！"));
                singlePicMsgList.Add(new ElementAt(userId));

                await GlobalObj.WSRobotClient.SendMessageAsync(
                    MessageType.group_,
                    groupId, new Message(singlePicMsgList.ToArray()));

                File.Delete(tmpFileName);
            }
            else
            {
                MessageHelper.SendGroupTextMessageWithAt(groupId, userId, $"范围指定有误！该作品只有{imageCount}页！");
            }

        }

        public static async Task GetUserIllustListAsync(long groupId, long userId, string pixivUserId, int page = 1)
        {
            JObject jo = (JObject)JsonConvert.DeserializeObject(HttpHelper.HttpGet(ImjadUrl_v2 + $"?type=member_illust&id={pixivUserId}&page={page}"));
            string username = jo["illusts"][0]["user"]["name"].ToString();
            string avatarUrl = jo["illusts"][0]["user"]["profile_image_urls"]["medium"].ToString();

            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
            string tmpAvatarFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());
            HttpHelper.DownloadFile(avatarUrl, tmpAvatarFileName, HttpHelper.PixivRefererUrl);

            byte[] imgBytesIn;
            using (FileStream fs = new(tmpAvatarFileName, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new(fs);
                imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                fs.Close();
                br.Dispose();
            }

            MemoryStream ms = new();
            ImageProcessHelper.Resize(Image.Load(imgBytesIn), 64, 64).SaveAsJpeg(ms);

            List<Element> messageContentList = new();
            List<string> tmpThumbnailPathList = new();

            int illustCount = 0;
            int proIllustCount = 0;
            int splitPageCount = 0;

            messageContentList.Add(new ElementImage(ms.ToArray()));
            messageContentList.Add(new ElementText($"画师：{username}（{pixivUserId}）的作品如下:"));

            foreach (var item in jo["illusts"])
            {
                if (proIllustCount == 10)
                {
                    splitPageCount++;
                    proIllustCount = 0;
                    messageContentList.Add(new ElementText($"\n由于数据过多，当前并没有展示完所有作品哦~请稍候，我正在为您处理下一部分……（当前为第{splitPageCount}部分，已展示共计{illustCount}个作品）"));
                    await GlobalObj.WSRobotClient.SendMessageAsync(
                        MessageType.group_,
                        groupId, new Message(messageContentList.ToArray()));
                    messageContentList = new List<Element>
                    {
                        new ElementText("（接上条消息）")
                    };
                    foreach (var path in tmpThumbnailPathList)
                    {
                        File.Delete(path);
                    }
                    tmpThumbnailPathList = new List<string>();
                }
                messageContentList.Add(new ElementText($"\n标题:{item["title"]}{(item["x_restrict"].ToInt() == 0 ? "" : "（R18）")}，ID:{item["id"]}"));
                if (item["x_restrict"].ToInt() == 0)
                {
                    const int resizeHeight = 128;
                    string tmpThumbnailFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());
                    HttpHelper.DownloadFile(item["image_urls"]["square_medium"].ToString(), tmpThumbnailFileName, HttpHelper.PixivRefererUrl);

                    tmpThumbnailPathList.Add(tmpThumbnailFileName);

                    List<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(tmpThumbnailFileName).ToList();

                    var fileTypeItem = directories.Find(e => e.Name.Equals("File Type", StringComparison.Ordinal));
                    var tagList = fileTypeItem.Tags.ToList();
                    if (tagList.Find(e => e.Name.Equals("Detected File Type Name", StringComparison.Ordinal)).Description.Equals("WebP", StringComparison.Ordinal))
                    {
                        //WebPExecuteDownloader.Download();

                        var builder = new WebPDecoderBuilder();

                        var encoder = builder
                            .Resize(resizeHeight, 0) // 調整寬度為32，等比縮放(因為高度為0)
                            .Build(); // 建立解碼器
                        string outputTmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());

                        using (var outputFile = File.Open(outputTmpFileName, FileMode.Create))
                        using (var inputFile = File.Open(tmpThumbnailFileName, FileMode.Open))
                        {
                            encoder.Decode(inputFile, outputFile); // 解碼
                        }
                        FileStream tmpFs = new FileStream(outputTmpFileName, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行读取
                        BinaryReader tmpBr = new BinaryReader(tmpFs);
                        byte[] imgBytes = tmpBr.ReadBytes((int)tmpFs.Length); //将流读入到字节数组中
                        tmpFs.Close();
                        tmpBr.Dispose();

                        messageContentList.Add(new ElementImage(imgBytes));
                        File.Delete(outputTmpFileName);
                    }
                    else
                    {
                        byte[] imgBytesDl;
                        using (FileStream fs = new(tmpThumbnailFileName, FileMode.Open, FileAccess.Read))
                        {
                            BinaryReader br = new(fs);
                            imgBytesDl = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                            fs.Close();
                            br.Dispose();
                        }
                        string convertFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());
                        ImageProcessHelper.Resize(Image.Load(imgBytesDl), resizeHeight, 0).Save(convertFileName);
                        tmpThumbnailPathList.Add(convertFileName);

                        byte[] imgBytes;
                        using (FileStream tmpFs = new(convertFileName, FileMode.Open, FileAccess.Read))
                        {
                            BinaryReader tmpBr = new(tmpFs);
                            imgBytes = tmpBr.ReadBytes((int)tmpFs.Length); //将流读入到字节数组中
                            tmpFs.Close();
                            tmpBr.Dispose();
                        }

                        messageContentList.Add(new ElementImage(imgBytes));
                    }
                }
                illustCount++;
                proIllustCount++;
            }

            messageContentList.Add(new ElementText($"\n以上共有{illustCount}个作品，当前为第{page}页，使用P站找图功能即可找出以上图的相关详情！" +
                $"\n※Pixiv画师查询功能由Cyrus API Hub驱动，图片来自Pixiv，版权归原作者所有"));

            await GlobalObj.WSRobotClient.SendMessageAsync(
                MessageType.group_,
                groupId, new Message(messageContentList.ToArray()));

            foreach (var item in tmpThumbnailPathList)
            {
                File.Delete(item);
            }
            File.Delete(tmpAvatarFileName);
        }
    }
}
