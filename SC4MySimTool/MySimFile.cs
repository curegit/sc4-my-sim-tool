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

		public static void Reorder(int source, int destination)
		{
			CreateMySimFileIfNotExists();
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Open, FileAccess.ReadWrite))
				{
					var sourceHead = 0;
					var sourceLength = 0;
					var destinationPosition = 0;
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);
					var b = bytes.Skip(4);
					var i = 0;
					var head = 4;
					for (var len = 0; b.Count() > 0; i++, head += len, len = 0)
					{
						len += 1;
						var nameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						len += nameLength;
						b = b.Skip(nameLength);
						len += 2;
						b = b.Skip(2);
						len += 1;
						var filenameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						len += filenameLength;
						b = b.Skip(filenameLength);
						if (i == destination)
						{
							destinationPosition = head - sourceLength;
						}
						if (i == source)
						{
							sourceHead = head;
							sourceLength = len;
						}
					}
					if (i == destination)
					{
						destinationPosition = head - sourceLength;
					}
					if (sourceHead == 0)
					{
						throw new InvalidOperationException("Source index is out of range.");
					}
					if (destinationPosition == 0)
					{
						throw new InvalidOperationException("Destination index is out of range.");
					}
					var moving = bytes.Skip(sourceHead).Take(sourceLength).ToArray();
					var deleted = bytes.Take(sourceHead).Concat(bytes.Skip(sourceHead + sourceLength)).ToArray();
					var array = deleted.Take(destinationPosition).Concat(moving).Concat(deleted.Skip(destinationPosition)).ToArray();
					stream.Position = 0;
					stream.Write(array, 0, array.Length);
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
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Open, FileAccess.Read))
				{
					var count = 0;
					stream.Seek(4, SeekOrigin.Current);
					while (stream.Position != stream.Length)
					{
						var nameLength = stream.ReadByte();
						var nameBytes = new byte[nameLength];
						stream.Read(nameBytes, 0, nameLength);
						var nameString = DecodeUTF8(nameBytes);
						var gender = (Gender)stream.ReadByte();
						var sign = (ZodiacSign)stream.ReadByte();
						var filenameLength = stream.ReadByte();
						var filenameBytes = new byte[filenameLength];
						stream.Read(filenameBytes, 0, filenameLength);
						var filenameString = DecodeUTF8(filenameBytes);
						Console.WriteLine($"{nameString} ({gender}) : {sign} [{filenameString}.bmp][{count}]");
						count++;
					}
				}
			}
			catch
			{
				throw new IOException("Can't read MySims.dat file.");
			}
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
