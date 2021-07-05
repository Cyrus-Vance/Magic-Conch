using MetadataExtractor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagicConchQQRobot.Modules.Utils
{
	class ImageProcessHelper
	{
		public static byte[] FunnyGIFConvert(string filePath)
		{
			FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
			BinaryReader br = new BinaryReader(fs);
			byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
			fs.Close();

			// Load the resized image that will be added
			using var image = Image.Load(imgBytesIn);
			Image flipImage = Image.Load(imgBytesIn);

			flipImage.Mutate(x => x.Flip(FlipMode.Horizontal));

			var metaData_source = image.Frames[0].Metadata;
			GifFrameMetadata gifMeta_source = metaData_source.GetFormatMetadata(GifFormat.Instance);
			gifMeta_source.FrameDelay = 10;
			//gifMeta.ColorTableLength = 128;
			gifMeta_source.DisposalMethod = GifDisposalMethod.RestoreToBackground;

			var metaData_flip = flipImage.Frames[0].Metadata;
			GifFrameMetadata gifMeta_flip = metaData_flip.GetFormatMetadata(GifFormat.Instance);
			gifMeta_flip.FrameDelay = 10;
			//gifMeta_flip.ColorTableLength = 128;
			gifMeta_flip.DisposalMethod = GifDisposalMethod.RestoreToBackground;

			// Add the image to the gif
			image.Frames.AddFrame(flipImage.Frames[0]);

			var imageMetadata = image.Metadata;
			GifMetadata gifMetadata = imageMetadata.GetGifMetadata();
			gifMetadata.RepeatCount = 0;

			MemoryStream ms = new MemoryStream();

			image.SaveAsGif(ms);

			return ms.ToArray();
		}
		public static Image Resize(Image input, int width, int height)
		{
			//Clone会返回一个经过处理的深度拷贝的image对象. 
			return input.Clone(x => x.Resize(width, height));
			/* 直接处理用Mutate=>Action
			input.Mutate(x =>
			{
				//直接处理image对象
				x.Resize(width, height);
			});
			return input;
			*/
		}

		public static byte[] StreamToBytes(Stream stream)
		{
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			// 设置当前流的位置为流的开始 
			stream.Seek(0, SeekOrigin.Begin);
			return bytes;
		}

		public static string GetFileRealExtName(string path)
		{
			List<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(path).ToList();

			var fileTypeItem = directories.Find(e => e.Name.Equals("File Type", StringComparison.Ordinal));
			var tagList = fileTypeItem.Tags.ToList();

			return tagList.Find(e => e.Name.Equals("Expected File Name Extension", StringComparison.Ordinal)).Description;
		}
	}
}
