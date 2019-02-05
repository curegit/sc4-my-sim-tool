using System;
using System.IO;
using System.Linq;
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
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Open, FileAccess.ReadWrite))
				{
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);
					var b = bytes.Skip(4);
					for (int len = 0, head = 4; b.Count() > 0; head += len, len = 0)
					{
						len += 1;
						var nameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						len += nameLength;
						var nameBytes = b.Take(nameLength).ToArray();
						var nameString = DecodeUTF8(nameBytes);
						b = b.Skip(nameLength);
						len += 2;
						b = b.Skip(2);
						len += 1;
						var filenameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						var filenameBytes = b.Take(filenameLength).ToArray();
						var filenameString = DecodeUTF8(filenameBytes);
						len += filenameLength;
						if (name == nameString)
						{
							var array = bytes.Take(head).Concat(bytes.Skip(head + len)).ToArray();
							stream.Position = 0;
							stream.Write(array, 0, array.Length);
							stream.SetLength(array.Length);
							try
							{
								File.Delete(MySimFolderPath + @"\" + filenameString + ".bmp");
							}
							catch
							{
								throw new Exception("Removed the Sim from MySims.dat file. But failed to delete the bitmap image file.");
							}
							return;
						}
						b = b.Skip(filenameLength);
					}
					throw new InvalidOperationException("No such Sim.");
				}
			}
			catch (IOException)
			{
				throw new IOException("Can't read from or write to MySims.dat file.");
			}
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
				throw new IOException("Can't save a image file. A Sim with the same name may already exist.");
			}
		}

		public static void Show()
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
