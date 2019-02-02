using System;
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
			var img = SaveImage(mySim.Bitmap, mySim.ImageFileName);
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Append, FileAccess.Write))
				{
					stream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (IOException)
			{
				try
				{
					File.Delete(img);
				}
				finally
				{
					throw new IOException("Can't write to MySims.dat file.");
				}
			}
		}

		public static void Remove(string name)
		{
			CreateMySimFileIfNotExists();

		}

		private static string SaveImage(Bitmap bitmap, string fileName)
		{
			try
			{
				var path = MySimFolderPath + @"\" + fileName + ".bmp";
				using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
				{
					bitmap.Save(stream, ImageFormat.Bmp);
				}
				return path;
			}
			catch (IOException)
			{
				throw new IOException("Can't save a image file. ");
			}
		}

		public static void Dump()
		{
			CreateMySimFileIfNotExists();
		}

		private static void CreateMySimFileIfNotExists()
		{
			try
			{
				if (!File.Exists(MySimFilePath))
				{
					using (var stream = File.OpenWrite(MySimFilePath))
					{
						stream.Write(FileType, 0, FileType.Length);
					}
				}
			}
			catch (IOException)
			{
				throw new IOException("Can't create MySims.dat file.");
			}
		}

		private static string DecodeUTF8(byte[] bytes)
		{
			var unicode = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, bytes);
			return Encoding.Unicode.GetString(unicode);
		}
	}
}
