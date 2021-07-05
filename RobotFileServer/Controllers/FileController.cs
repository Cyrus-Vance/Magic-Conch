using MetadataExtractor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RobotFileServer.Globals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Directory = System.IO.Directory;

namespace RobotFileServer.Controllers
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        /// <summary>
        /// 通过URL获取图片
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        [HttpGet("image-url")]
        public ActionResult UploadImageByUrl(string fileUrl)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"file"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"file"));
            string generatedFileGuid = Guid.NewGuid().ToString();
            string downloadFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", generatedFileGuid);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileUrl);
            request.Method = "GET";
            request.Referer = "https://www.pixiv.net";
            //request.UserAgent = GlobalSet.UserAgent_Chrome;
            request.ContentType = "text/html;charset=UTF-8";
            //request.CookieContainer = GlobalObj.GlobalCookies;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            long fileSize = response.ContentLength;
            Stream myResponseStream = response.GetResponseStream();
            Stream fileSaveStream = new FileStream(downloadFileName, FileMode.OpenOrCreate, FileAccess.Write);
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
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]已写入1024字节,{bytescount}/{fileSize}");
                }
            }
            catch
            {
                Console.WriteLine($"图片下载发生错误！！！");
            }
            fileSaveStream.Flush();
            fileSaveStream.Close();

            List<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(downloadFileName).ToList();

            var fileTypeItem = directories.Find(e => e.Name.Equals("File Type", StringComparison.Ordinal));
            var tagList = fileTypeItem.Tags.ToList();
            string newGeneratedFileName = generatedFileGuid + '.' + tagList.Find(e => e.Name.Equals("Expected File Name Extension", StringComparison.Ordinal)).Description;
            System.IO.File.Move(downloadFileName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", newGeneratedFileName));


            FileCleaner.AddCleanFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"file", newGeneratedFileName), GlobalSet.FileExpireSeconds);

            return Ok(new { url = newGeneratedFileName, expire = GlobalSet.FileExpireSeconds });
        }


        /// <summary>
        /// 上传图片文件
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost("image")]
        public async Task<ActionResult> UploadImageFileAsync(IFormFile uploadFile)
        {
            if (uploadFile != null && uploadFile.Length > 0)
            {
                var allowType = new string[] { "image/jpg", "image/png", "image/jpeg" };
                string contentType = uploadFile.ContentType;
                if (allowType.Contains(contentType) || true)
                {
                    long size = uploadFile.Length;

                    if (size > 0)
                    {
                        if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"))) Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"cache"));
                        string tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", Guid.NewGuid().ToString());

                        using var stream = System.IO.File.Create(tmpFileName);
                        await uploadFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    // Process uploaded files
                    // Don't rely on or trust the FileName property without validation.

                    return Ok();
                }
            }
            return Ok();
        }

        [HttpGet("image")]
        public FileContentResult GetImageByFileName(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", fileName);
            if (System.IO.File.Exists(filePath))
            {
                List<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(filePath).ToList();

                var fileTypeItem = directories.Find(e => e.Name.Equals("File Type", StringComparison.Ordinal));
                var tagList = fileTypeItem.Tags.ToList();

                string extName = tagList.Find(e => e.Name.Equals("Expected File Name Extension", StringComparison.Ordinal)).Description;

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行读取
                BinaryReader br = new BinaryReader(fs);
                byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                fs.Close();
                return File(imgBytesIn, "image/" + extName);
            }
            else
            {
                string outdatedImgFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image", "outdated.png");
                FileStream fs = new FileStream(outdatedImgFilePath, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行读取
                BinaryReader br = new BinaryReader(fs);
                byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                fs.Close();
                return File(imgBytesIn, "image/png");
            }
            
        }
    }
}
