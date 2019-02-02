using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace SC4MySimTool
{
	public static class MySimFile
	{
		private static readonly byte[] FileType = { 0xAA, 0xE4, 0x32, 0x4A };

		private static readonly string MySimFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SimCity 4\MySim";

		private static readonly string MySimFilePath = MySimFolderPath + @"\MySims.dat";

		public static void Add(MySim mySim)
		{
			CreateMySimFileIfNotExists();
			var bytes = mySim.Encode();
			SaveImage(mySim.Bitmap, mySim.ImageFileName);
			using (var stream = new FileStream(MySimFilePath, FileMode.Append, FileAccess.Write))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		public static void Remove(string name)
		{
			CreateMySimFileIfNotExists();

		}

		private static void SaveImage(Bitmap bitmap, string fileName)
		{
			using (var stream = new FileStream(MySimFolderPath + @"\" + fileName + ".bmp", FileMode.CreateNew, FileAccess.Write))
			{
				bitmap.Save(stream, ImageFormat.Bmp);
			}
		}

		public static void Dump()
		{
			CreateMySimFileIfNotExists();
		}

		private static void CreateMySimFileIfNotExists()
		{
			if (!File.Exists(MySimFilePath))
			{
				using (var stream = File.OpenWrite(MySimFilePath))
				{
					stream.Write(FileType, 0, FileType.Length);
				}
			}
		}

		private static string DecodeUTF8(byte[] bytes)
		{
			var unicode = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, bytes);
			return Encoding.Unicode.GetString(unicode);
		}
	}
}
