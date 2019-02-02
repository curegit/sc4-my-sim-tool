using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;

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
			Name = name ?? throw new ArgumentNullException();
			Gender = gender;
			ZodiacSign = zodiacSign;
			Bitmap = ImportImage(imageFilePath);
			ImageFileName = GenerateUniqueFileName(Name);
		}

		public byte[] Encode()
		{
			var nameBytes = GetUTF8(Name);
			if (nameBytes.Length > 255) throw new Exception(); 
			var nameLength = (byte)nameBytes.Length;
			var gender = (byte)Gender;
			var zodiac = (byte)ZodiacSign;
			var fileNameBytes = GetUTF8(ImageFileName);
			if (fileNameBytes.Length > 255) throw new Exception();
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

		private static Bitmap ImportImage(string path)
		{
			var source = new Bitmap(path);
			var destination = new Bitmap(36, 41);
			var graphics = Graphics.FromImage(destination);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.DrawImage(source, 0, 0, 36, 41);
			graphics.Dispose();
			source.Dispose();
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

		private static string GenerateUniqueFileName(string name)
		{
			var sha = new SHA512CryptoServiceProvider();
			var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(name));
			var base64 = Convert.ToBase64String(hash, Base64FormattingOptions.None);
			return base64.Replace('/', '-').Substring(0, 12);
		}

		private static byte[] GetUTF8(string str)
		{
			byte[] unicode = Encoding.Unicode.GetBytes(str);
			return Encoding.Convert(Encoding.Unicode, Encoding.UTF8, unicode);
		}
	}
}
