using MagicConchQQRobot.DataObjs;
using MagicConchQQRobot.DataObjs.DbClass;
using MagicConchQQRobot.Globals;
using MagicConchQQRobot.Modules.Utils;
using CyrusVance.CoolQRobot.ApiCall.Requests;
using CyrusVance.CoolQRobot.Enums;
using CyrusVance.CoolQRobot.Messages.CQElements;
using CyrusVance.CoolQRobot.Messages.CQElements.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Directory = System.IO.Directory;

namespace MagicConchQQRobot.Modules.InteractiveProvider
{
    class FunnyImage
    {
        public static readonly string XianbeiImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image", "xianbei");
        public static readonly string DragonImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image", "lyt");

        private static readonly List<string> AllowImageExt = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };

        private const string ExistedImage = "这张图已经有了，请换一张~";

        public enum ImageType
        {
            Unknown = 0,
            Xianbei = 1,
            Lyt = 2
        }

        public static Dictionary<ImageType, string> ImageTypeTextDictionary = new Dictionary<ImageType, string>
        {
            { ImageType.Xianbei,"先辈粪图" },
            { ImageType.Lyt,"龙图" }
        };

        public static void CreateDirectory()
        {
            if (!Directory.Exists(XianbeiImagePath)) Directory.CreateDirectory(XianbeiImagePath);
            if (!Directory.Exists(DragonImagePath)) Directory.CreateDirectory(DragonImagePath);
        }

        public static void CheckAllImageDirectory(long groupId)
        {
            string sendText = "";
            DirectoryInfo xianbeiDir = new DirectoryInfo(XianbeiImagePath);
            FileInfo[] xianbeiFileInfos = xianbeiDir.GetFiles();
            sendText += $"{ImageTypeTextDictionary[ImageType.Xianbei]}图库中共有{xianbeiFileInfos.Length}个图片，{ImagehashXianbei.FindCount()}个数据库项.";

            DirectoryInfo lytDir = new DirectoryInfo(DragonImagePath);
            FileInfo[] lytFileInfos = lytDir.GetFiles();
            sendText += $"\n{ImageTypeTextDictionary[ImageType.Lyt]}图库中共有{lytFileInfos.Length}个图片，{ImagehashLyt.FindCount()}个数据库项.";

            MessageHelper.SendGroupTextMessage(groupId, sendText);
        }

        public static void ClearAllImages(ImageType type, long groupId)
        {
            int cleanFileCount = 0;
            int cleanDatabaseCount = 0;
            switch (type)
            {
                case ImageType.Xianbei:
                    {
                        DirectoryInfo directory = new DirectoryInfo(XianbeiImagePath);
                        FileInfo[] fileInfos = directory.GetFiles();
                        foreach (var item in fileInfos)
                        {
                            File.Delete(item.FullName);
                        }

                        var images = ImagehashXianbei.FindAll();
                        foreach (var item in images)
                        {
                            item.Delete();
                        }
                        cleanFileCount = fileInfos.Length;
                        cleanDatabaseCount = images.Count;
                        break;
                    }
                case ImageType.Lyt:
                    {
                        DirectoryInfo directory = new DirectoryInfo(DragonImagePath);
                        FileInfo[] fileInfos = directory.GetFiles();
                        foreach (var item in fileInfos)
                        {
                            File.Delete(item.FullName);
                        }

                        var images = ImagehashLyt.FindAll();
                        foreach (var item in images)
                        {
                            item.Delete();
                        }
                        cleanFileCount = fileInfos.Length;
                        cleanDatabaseCount = images.Count;
                        break;
                    }
            }

            MessageHelper.SendGroupTextMessage(groupId, $"{ImageTypeTextDictionary[type]}已经被全部清理，共清理了{cleanFileCount}个图片，{cleanDatabaseCount}个数据库项！");
        }

        public static void RapeSuNBMember(long groupId, string memberCode, int rapeLevel = 0)
        {
            Member member = Member.Find(Member._.Code == memberCode);


            List<Element> elements = new List<Element>();
            byte[] imgBytes = TryFindPhoto(ImageType.Xianbei);
            if (imgBytes != null)
            {
                elements.Add(new ElementImage(imgBytes));
            }

            string rapeText = $"{memberCode}被先辈灌下了昏睡红茶并且被无情地雷普了...";
            switch (rapeLevel)
            {
                case 1:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, member.qq, 604800)).Wait();
                        rapeText = $"{memberCode}被先辈灌下了奇怪的昏睡红茶并且被无情地雷普了...{memberCode}被雷普以后睡得很香，张着嘴巴闭着眼...";
                        break;
                    }
                case 2:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, member.qq, 1209600)).Wait();
                        rapeText = $"{memberCode}被先辈灌下了奇怪的昏睡红茶并且被无情地雷普了...{memberCode}被雷普以后睡得很香，张着嘴巴闭着眼,仿佛已经到了人生的巅峰...";
                        break;
                    }
                default:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, member.qq, 172800)).Wait();
                        break;
                    }
            }
            elements.Add(new ElementText(rapeText));
            GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId, new CyrusVance.CoolQRobot.Messages.Message(elements.ToArray())).GetAwaiter().GetResult();
        }

        public static void RapeGroupMember(long groupId, string memberName, long raperQQ, int rapeLevel, bool rapeDirectly = false, GroupUser rapedUser = null)
        {
            string rapingUserName = string.Empty;
            long rapingUserQQ = 0;
            if (!rapeDirectly)
            {
                List<GroupUser> resultList = (from c in GlobalObj.GroupRobotUserLists[groupId] where c.GroupNickname.ToLower().Contains(memberName) || c.Nickname.ToLower().Contains(memberName) select c).ToList();

                if (resultList.Count == 1)
                {
                    rapingUserName = string.IsNullOrEmpty(resultList[0].GroupNickname) ? resultList[0].Nickname : resultList[0].GroupNickname;
                    rapingUserQQ = resultList[0].QQNumber;
                    SendBanRequest(groupId, rapingUserQQ, rapingUserName, rapeLevel);
                }
                else if (resultList.Count > 1)
                {
                    GlobalObj.GroupRobotUserLists[groupId][GlobalObj.GroupRobotUserLists[groupId].FindIndex(e => e.QQNumber == raperQQ)].RapeUserList = resultList;
                    GlobalObj.GroupRobotUserLists[groupId][GlobalObj.GroupRobotUserLists[groupId].FindIndex(e => e.QQNumber == raperQQ)].RapeLevel = rapeLevel;

                    string sendMessageText = "找到多个可能被您雷普的对象：";
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        sendMessageText += Environment.NewLine + $"ID:{i},昵称:{(string.IsNullOrEmpty(resultList[i].Nickname) ? "空昵称" : resultList[i].Nickname)}，群名片:{(string.IsNullOrEmpty(resultList[i].GroupNickname) ? "空群名片" : resultList[i].GroupNickname)}({resultList[i].QQNumber})";
                    }
                    sendMessageText += Environment.NewLine + "输入/leipu [编号]来雷普您需要雷普的对象吧！";
                    MessageHelper.SendGroupTextMessage(groupId, sendMessageText);
                }
                else
                {
                    MessageHelper.SendGroupTextMessage(groupId, "没有找到有叫" + memberName + "这个名字的群员！");
                }
            }
            else
            {
                SendBanRequest(groupId, rapedUser.QQNumber, string.IsNullOrEmpty(rapedUser.GroupNickname) ? rapedUser.Nickname : rapedUser.GroupNickname, rapeLevel);
            }
        }

        private static void SendBanRequest(long groupId, long rapingUserQQ, string rapingUserName, int rapeLevel)
        {
            List<Element> elements = new List<Element>();
            byte[] imgBytes = TryFindPhoto(ImageType.Xianbei);
            if (imgBytes != null)
            {
                elements.Add(new ElementImage(imgBytes));
            }

            string rapeText = $"{rapingUserName}被先辈灌下了昏睡红茶并且被无情地雷普了...";
            switch (rapeLevel)
            {
                case 1:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, rapingUserQQ, 604800)).Wait();
                        rapeText = $"{rapingUserName}被先辈灌下了奇怪的昏睡红茶并且被无情地雷普了...{rapingUserName}被雷普以后睡得很香，张着嘴巴闭着眼...";
                        break;
                    }
                case 2:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, rapingUserQQ, 1209600)).Wait();
                        rapeText = $"{rapingUserName}被先辈灌下了奇怪的昏睡红茶并且被无情地雷普了...{rapingUserName}被雷普以后睡得很香，张着嘴巴闭着眼,仿佛已经到了人生的巅峰...";
                        break;
                    }
                default:
                    {
                        GlobalObj.WSRobotClient.SendRequestAsync(new SetGroupBanRequest(groupId, rapingUserQQ, 172800)).Wait();
                        break;
                    }
            }
            elements.Add(new ElementText(rapeText));
            GlobalObj.WSRobotClient.SendMessageAsync(MessageType.group_, groupId, new CyrusVance.CoolQRobot.Messages.Message(elements.ToArray())).GetAwaiter().GetResult();

        }

        public static byte[] TryFindPhoto(ImageType type)
        {
            int failedCount = 0;
            string findingPath = "";

            switch (type)
            {
                case ImageType.Xianbei:
                    {
                        findingPath = XianbeiImagePath;
                        break;
                    }
                case ImageType.Lyt:
                    {
                        findingPath = DragonImagePath;
                        break;
                    }
            }

            DirectoryInfo directory = new DirectoryInfo(findingPath);
            FileInfo[] fileInfos = directory.GetFiles();

            if (fileInfos.Length == 0) return null;
            while (true)
            {
                Random rnd = new Random();
                int rndIndex = rnd.Next(fileInfos.Length);

                Console.WriteLine($"正在发送随机图片，图库：{(int)type},文件路径：{fileInfos[rndIndex].FullName}");

                byte[] imgBytes;
                if (AllowImageExt.Contains(fileInfos[rndIndex].Extension))
                {
                    using FileStream tmpFs = new FileStream(fileInfos[rndIndex].FullName, FileMode.Open, FileAccess.Read);
                    BinaryReader tmpBr = new BinaryReader(tmpFs);
                    imgBytes = tmpBr.ReadBytes((int)tmpFs.Length); //将流读入到字节数组中
                    tmpFs.Close();
                    tmpBr.Dispose();
                    return imgBytes;
                }
                else
                {
                    failedCount++;
                }

                if (failedCount == 3)
                {
                    return null;
                }
            }
        }

        public static void LearnImages(ImageType type, long groupId, long userId, string imageUrl)
        {
            string downloadRootPath = "";
            switch (type)
            {
                case ImageType.Xianbei:
                    {
                        downloadRootPath = XianbeiImagePath;
                        break;
                    }
                case ImageType.Lyt:
                    {
                        downloadRootPath = DragonImagePath;
                        break;
                    }
            }
            string savePath = Path.Combine(downloadRootPath, Guid.NewGuid().ToString());
            HttpHelper.DownloadFile(imageUrl, savePath);

            string newGeneratedName = Guid.NewGuid().ToString() + '.' + ImageProcessHelper.GetFileRealExtName(savePath);
            string newPath = Path.Combine(downloadRootPath, newGeneratedName);
            File.Move(savePath, newPath);

            string hashSHA1 = FileHashHelper.CalcFileHash(newPath);

            switch (type)
            {
                case ImageType.Xianbei:
                    {
                        if (ImagehashXianbei.FindByHash(hashSHA1) != null)
                        {
                            File.Delete(newPath);
                            MessageHelper.SendGroupTextMessage(groupId, ExistedImage);
                            return;
                        }
                        else
                        {
                            ImagehashXianbei imagehashXianbei = new ImagehashXianbei
                            {
                                Hash = hashSHA1,
                                Filename = newGeneratedName,
                                HashType = 1,
                                UploadTime = TimestampHelper.GetTimestampSecond(),
                                Uid = userId
                            };
                            imagehashXianbei.Insert();
                        }
                        break;
                    }
                case ImageType.Lyt:
                    {
                        if (ImagehashLyt.FindByHash(hashSHA1) != null)
                        {
                            File.Delete(newPath);
                            MessageHelper.SendGroupTextMessage(groupId, ExistedImage);
                            return;
                        }
                        else
                        {
                            ImagehashLyt imagehashLyt = new ImagehashLyt
                            {
                                Hash = hashSHA1,
                                Filename = newGeneratedName,
                                HashType = 1,
                                UploadTime = TimestampHelper.GetTimestampSecond(),
                                Uid = userId
                            };
                            imagehashLyt.Insert();
                        }
                        break;
                    }
            }

            DirectoryInfo directory = new DirectoryInfo(downloadRootPath);
            FileInfo[] fileInfos = directory.GetFiles();

            MessageHelper.SendGroupTextMessage(groupId, $"学习成功！{ImageTypeTextDictionary[type]}图库已有{fileInfos.Length}张图片，欢迎持续施工~");

        }
    }
}
