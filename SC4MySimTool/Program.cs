using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SC4MySimTool
{
	public class Program
	{
		private static int Main(string[] args)
		{
			try
			{
				if (args.Length > 0)
				{
					switch (args[0])
					{
						case "help":
							ShowHelp();
							break;
						case "add":
							AddMySim(args.Skip(1).ToArray());
							Console.WriteLine("The operation was completed successfully.");
							break;
						case "remove":
							RemoveMySim(args.Skip(1).ToArray());
							Console.WriteLine("The operation was completed successfully.");
							break;
						case "show":
							ShowMySims();
							break;
						default:
							if (File.Exists(args[0])) ReadFromStandardIO(args[0]);
							else ShowHelp();
							break;
					}
				}
				else
				{
					ReadFromStandardIO();
				}
				Console.WriteLine("Press any key to quit.");
				Console.ReadKey();
				return 0;
			}
			catch (Exception e)
			{
				Console.Write("Exception occurred: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Press any key to quit.");
				Console.ReadKey();
				return -1;
			}
		}

		private static void ReadFromStandardIO(string imageFilepath = null)
		{
			while (true)
			{
				string command;
				if (imageFilepath == null)
				{
					Console.WriteLine("Type 'add', 'remove', 'show', or 'help'.");
					command = Console.ReadLine();
				}
				else
				{
					command = "add";
				}
				switch (command)
				{
					case "add":
NameEntry:
						Console.WriteLine("Enter a name of a new Sim.");
						var newName = Console.ReadLine();
						if (newName == "") goto NameEntry;
GenderEntry:
						Console.WriteLine("Enter gender of the Sim. Type 'male' or 'female'.");
						var gender = Console.ReadLine();
						try
						{
							ParseGender(gender);
						}
						catch
						{
							goto GenderEntry;
						}
SignEntry:
						Console.WriteLine("Enter sign of the Sim. Type 'aquarius', 'aries', 'cancer', 'capricorn', 'gemini', 'leo', 'libra', 'pisces', 'sagittarius', 'scorpio', 'taurus', or 'virgo'.");
						var sign = Console.ReadLine();
						try
						{
							ParseZodiacSign(sign);
						}
						catch
						{
							goto SignEntry;
						}
ImageFileEntry:
						if (imageFilepath != null)
						{
							AddMySim(new string[] { newName, gender, sign, imageFilepath });
							Console.WriteLine("The operation was completed successfully.");
							break;
						}
						Console.WriteLine("Enter filepath of a image of the Sim.");
						var filepath = Console.ReadLine();
						if (filepath == "") goto ImageFileEntry;
						AddMySim(new string[] { newName, gender, sign, filepath });
						Console.WriteLine("The operation was completed successfully.");
						break;
					case "remove":
						while (true)
						{
							Console.WriteLine("Type the name of a Sim you want to remove.");
							var name = Console.ReadLine();
							if (name != "")
							{
								RemoveMySim(new string[] { name });
								break;
							}
						}
						Console.WriteLine("The operation was completed successfully.");
						break;
					case "show":
						ShowMySims();
						break;
					case "help":
						ShowHelp();
						break;
					default:
						continue;
				}
				break;
			}
		}

		private static void ShowHelp()
		{
			Console.WriteLine($@"
======================================
This is SC4 My Sim Tool ({Assembly.GetExecutingAssembly().GetName().Version})
======================================

    Commands
    --------
    [None]:
      Prompt inputs in stdin.

    help:
      Show this help.

    show:
      Show the My Sim list.

    add <name> <gender> <sign> <image_path>:
      Add a new Sim to the list.
      <gender> = female | male
      <sign> = aquarius | aries | cancer | capricorn | gemini | leo |
               libra | pisces | sagittarius | scorpio | taurus | virgo

    remove <name>:
      Remove a specified Sim from the list and delete its image file.


    Drag and Drop
    -------------
    Drag and drop a image file to the exe to add a new Sim using its image.

======================================
");
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
				throw new ArgumentException("'add' command takes 4 arguments.");
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
				throw new ArgumentException("'remove' command takes 1 argument.");
			}
		}

		private static void ShowMySims()
		{
			MySimFile.Show();
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
