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

		private static readonly string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) == "" ? throw new DirectoryNotFoundException("MyDocuments not found.") : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		private static readonly string MySimFolderPath = Path.Combine(MyDocumentsPath, "SimCity 4", "MySim");

		private static readonly string MySimFilePath = Path.Combine(MySimFolderPath, "MySims.dat");

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

		public static void Remove(int index)
		{
			CreateMySimFileIfNotExists();
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Open, FileAccess.ReadWrite))
				{
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);
					var b = bytes.Skip(4);
					for (int len = 0, head = 4, i = 0; b.Count() > 0; head += len, len = 0, i++)
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
						var filenameBytes = b.Take(filenameLength).ToArray();
						var filenameString = DecodeUTF8(filenameBytes);
						len += filenameLength;
						if (i == index)
						{
							var array = bytes.Take(head).Concat(bytes.Skip(head + len)).ToArray();
							stream.Position = 0;
							stream.Write(array, 0, array.Length);
							stream.SetLength(array.Length);
							try
							{
								File.Delete(Path.Combine(MySimFolderPath, filenameString + ".bmp"));
							}
							catch
							{
								throw new Exception("Removed the Sim from MySims.dat file, but failed to delete its bitmap image file.");
							}
							return;
						}
						b = b.Skip(filenameLength);
					}
					throw new InvalidOperationException("The index is out of range.");
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
						throw new InvalidOperationException("The source index is out of range.");
					}
					if (destinationPosition == 0)
					{
						throw new InvalidOperationException("The destination index is out of range.");
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

		public static void UpdateImage(int index, string imageFileName)
		{
			CreateMySimFileIfNotExists();
			var bitmap = MySim.ImportImage(imageFileName);
			string filename = null;
			try
			{
				using (var stream = new FileStream(MySimFilePath, FileMode.Open, FileAccess.Read))
				{
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);
					var b = bytes.Skip(4);
					for (var i = 0; b.Count() > 0; i++)
					{
						var nameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						b = b.Skip(nameLength);
						b = b.Skip(2);
						var filenameLength = (int)b.ElementAt(0);
						b = b.Skip(1);
						if (i == index)
						{
							filename = DecodeUTF8(b.Take(filenameLength).ToArray());
							break;
						}
						b = b.Skip(filenameLength);
					}
				}
			}
			catch (IOException)
			{
				throw new IOException("Can't read MySims.dat file.");
			}
			if (filename == null)
			{
				throw new InvalidOperationException("The index is out of range.");
			}
			SaveImage(bitmap, filename, true);
		}

		private static string SaveImage(Bitmap bitmap, string fileName, bool canOverwrite = false)
		{
			try
			{
				var path = Path.Combine(MySimFolderPath, fileName + ".bmp");
				using (var stream = new FileStream(path, canOverwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write))
				{
					bitmap.Save(stream, ImageFormat.Bmp);
				}
				Console.WriteLine($"Image Saved: {path}");
				return path;
			}
			catch (IOException)
			{
				throw new IOException("Couldn't save an image file. Please try again.");
			}
		}

		public static int Show(bool reorder)
		{
			CreateMySimFileIfNotExists();
			var defaultColor = Console.ForegroundColor;
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
						Console.ForegroundColor = ConsoleColor.Green;
						if (reorder) Console.WriteLine($"  + Destination: [{count}]");
						Console.ForegroundColor = defaultColor;
						Console.WriteLine($"[{count}] {nameString} ({gender}, {sign}) <{filenameString}.bmp>");
						count++;
					}
					if (count == 0) Console.WriteLine("No Sim.");
					else if (reorder)
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine($"  + Destination: [{count}]");
					}
					return count;
				}
			}
			catch
			{
				throw new IOException("Can't read MySims.dat file.");
			}
			finally
			{
				Console.ForegroundColor = defaultColor;
			}
		}

		private static void CreateMySimFileIfNotExists()
		{
			try
			{
				if (!File.Exists(MySimFilePath))
				{
					if (!Directory.Exists(MySimFolderPath))
					{
						Directory.CreateDirectory(MySimFolderPath);
					}
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
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
