using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace SC4MySimTool
{
	public class MySim
	{
		public string Name { get; }

		public Gender Gender { get; }

		public ZodiacSign ZodiacSign { get; }

		public string ImageFileName { get; }

		public Bitmap Bitmap { get; }

		public MySim(string name, Gender gender, ZodiacSign zodiacSign, string imageFilePath)
		{
			Name = name ?? throw new ArgumentNullException("'name' is required.");
			Gender = gender;
			ZodiacSign = zodiacSign;
			Bitmap = ImportImage(imageFilePath);
			ImageFileName = GenerateUniqueFileName(Name);
		}

		public byte[] Encode()
		{
			var nameBytes = GetUTF8(Name);
			if (nameBytes.Length > 255) throw new Exception("The name is too long.");
			var nameLength = (byte)nameBytes.Length;
			var gender = (byte)Gender;
			var zodiac = (byte)ZodiacSign;
			var fileNameBytes = GetUTF8(ImageFileName);
			if (fileNameBytes.Length > 255) throw new Exception("The image filename is too long.");
			var fileNameLength = (byte)fileNameBytes.Length;
			var array = new List<byte>();
			array.Add(nameLength);
			array.AddRange(nameBytes);
			array.Add(gender);
			array.Add(zodiac);
			array.Add(fileNameLength);
			array.AddRange(fileNameBytes);
			return array.ToArray();
		}

		public static Bitmap ImportImage(string path)
		{
			Bitmap source = null;
			if (path == "clipboard:" || path == "cb:")
			{
				var msPng = Clipboard.GetData("PNG") as MemoryStream;
				if (msPng != null)
				{
					try
					{
						msPng.Seek(0, SeekOrigin.Begin);
						source = new Bitmap(msPng);
					}
					catch { } finally
					{
						msPng.Dispose();
					}
				}
				if (source == null)
				{
					if (Clipboard.ContainsImage())
					{
						source = new Bitmap(Clipboard.GetImage());
					}
					else
					{
						throw new IOException("Can't import image from clipboard.");
					}
				}
			}
			try
			{
				source = source ?? new Bitmap(path);
				using (source)
				{
					var destination = new Bitmap(36, 41, PixelFormat.Format32bppArgb);
					using (var graphics = Graphics.FromImage(destination))
					{
						graphics.Clear(Color.White);
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
						graphics.DrawImage(source, 0, 0, 36, 41);
					}
					for (var i = 0; i < destination.Width; i++)
					{
						for (var j = 0; j < destination.Height; j++)
						{
							var color = destination.GetPixel(i, j);
							destination.SetPixel(i, j, color == Color.Magenta ? Color.FromArgb(254, 254, 0) : color);
						}
					}
					destination.SetPixel(0, 0, Color.Magenta);
					destination.SetPixel(1, 0, Color.Magenta);
					destination.SetPixel(0, 1, Color.Magenta);
					destination.SetPixel(34, 0, Color.Magenta);
					destination.SetPixel(35, 0, Color.Magenta);
					destination.SetPixel(35, 1, Color.Magenta);
					destination.SetPixel(0, 39, Color.Magenta);
					destination.SetPixel(0, 40, Color.Magenta);
					destination.SetPixel(1, 40, Color.Magenta);
					destination.SetPixel(34, 40, Color.Magenta);
					destination.SetPixel(35, 39, Color.Magenta);
					destination.SetPixel(35, 40, Color.Magenta);
					return destination;
				}
			}
			catch
			{
				throw new IOException("Can't import the image.");
			}
		}

		private static string GenerateUniqueFileName(string name)
		{
			var sha = new SHA256CryptoServiceProvider();
			var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(name + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffffff")));
			var base64 = Convert.ToBase64String(hash, Base64FormattingOptions.None);
			var valid = ValidFileName(base64);
			return valid.Substring(0, 12);
		}

		private static string ValidFileName(string name)
		{
			var str = name;
			foreach (var illegal in Path.GetInvalidFileNameChars())
			{
				str = str.Replace(illegal, '-');
			}
			return str;
		}

		private static byte[] GetUTF8(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}
	}
}
