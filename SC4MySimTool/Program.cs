using System;
using System.Linq;

namespace SC4MySimTool
{
	public class Program
	{
		private static int Main(string[] args)
		{
			string command = args.Length > 0 ? args[0] : "help";
			try
			{
				switch (command)
				{
					case "help":
						ShowHelp();
						break;
					case "add":
						AddMySim(args.Skip(1).ToArray());
						break;
					case "remove":
						RemoveMySim(args.Skip(1).ToArray());
						break;
					case "show":
						ShowMySims();
						break;
				}
				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception occurred:");
				Console.WriteLine(e.Message);
				return -1;
			}
		}

		private static void ShowHelp()
		{
			Console.WriteLine("help");
		}

		private static void AddMySim(string[] args)
		{
			if (args.Length == 4)
			{
				var name = args[0];
				var gender = ParseGender(args[1]);
				var zodiacSign = ParseZodiacSign(args[2]);
				var path = args[3];
				var sim = new MySim(name, gender, zodiacSign, path);
				MySimFile.Add(sim);
			}
			else
			{
				throw new ArgumentException("aaaa");
			}
		}

		private static void RemoveMySim(string[] args)
		{
			if (args.Length == 1)
			{
				MySimFile.Remove(args[0]);
			}
			else
			{
				throw new ArgumentException("");
			}
		}

		private static void ShowMySims()
		{
			MySimFile.Dump();
		}

		private static Gender ParseGender(string gender)
		{
			switch (gender)
			{
				case "female":
					return Gender.Female;
				case "male":
					return Gender.Male;
				default:
					throw new ArgumentException();
			}
		}

		private static ZodiacSign ParseZodiacSign(string zodiacSign)
		{
			switch (zodiacSign)
			{
				case "aquarius":
					return ZodiacSign.Aquarius;
				case "aries":
					return ZodiacSign.Aries;
				case "cancer":
					return ZodiacSign.Cancer;
				case "capricorn":
					return ZodiacSign.Capricorn;
				case "gemini":
					return ZodiacSign.Gemini;
				case "leo":
					return ZodiacSign.Leo;
				case "libra":
					return ZodiacSign.Libra;
				case "pisces":
					return ZodiacSign.Pisces;
				case "sagittarius":
					return ZodiacSign.Sagittarius;
				case "scorpio":
					return ZodiacSign.Scorpio;
				case "taurus":
					return ZodiacSign.Taurus;
				case "virgo":
					return ZodiacSign.Virgo;
				default:
					throw new ArgumentException();
			}
		}
	}
}
