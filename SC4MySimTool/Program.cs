using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;

namespace SC4MySimTool
{
	public class Program
	{
		[STAThread]
		private static int Main(string[] args)
		{
			if (args.Length > 0)
			{
				try
				{
					switch (args[0])
					{
						case "help":
						case "-h":
						case "--help":
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
						case "reorder":
							ReorderMySim(args.Skip(1).ToArray());
							Console.WriteLine("The operation was completed successfully.");
							break;
						case "update-image":
							UpdateSimsImage(args.Skip(1).ToArray());
							Console.WriteLine("The operation was completed successfully.");
							break;
						case "show":
							switch (args.Length)
							{
								case 1:
									ShowMySims();
									break;
								case 2:
									if (args[1] == "-r" || args[1] == "--reorder") ShowMySims(true);
									else throw new ArgumentException("An unknown option was passed.");
									break;
								default:
									throw new ArgumentException("An unknown option was passed.");
							}
							break;
						default:
							if (File.Exists(args[0]) || args[0] == "cb:" || args[0] == "clipboard:")
							{
								try
								{
									ReadFromStandardInput(args[0]);
									if (!Console.IsInputRedirected)
									{
										Console.WriteLine("Press any key to quit.");
										Console.ReadKey();
									}
									return 0;
								}
								catch (Exception e)
								{
									Console.Error.Write("An error occurred: ");
									Console.Error.WriteLine(e.Message);
									if (!Console.IsInputRedirected)
									{
										Console.WriteLine("Press any key to quit.");
										Console.ReadKey();
									}
									return -1;
								}
							}
							else
							{
								Console.Error.WriteLine($"'{args[0]}' is not a valid command.");
								ShowHelp();
								return -1;
							}
					}
					return 0;
				}
				catch (Exception e)
				{
					Console.Error.Write("An error occurred: ");
					Console.Error.WriteLine(e.Message);
					return -1;
				}
			}
			else
			{
				try
				{
					ReadFromStandardInput();
					if (!Console.IsInputRedirected)
					{
						Console.WriteLine("Press any key to quit.");
						Console.ReadKey();
					}
					return 0;
				}
				catch (Exception e)
				{
					Console.Error.Write("An error occurred: ");
					Console.Error.WriteLine(e.Message);
					if (!Console.IsInputRedirected)
					{
						Console.WriteLine("Press any key to quit.");
						Console.ReadKey();
					}
					return -1;
				}
			}
		}

		private static void ReadFromStandardInput(string imageFilepath = null)
		{
			while (true)
			{
				string command;
				Bitmap preloadedImage = null;
				if (imageFilepath == null)
				{
					Console.WriteLine("Type 'add', 'remove', 'reorder', 'update-image', 'show', or 'help'.");
					command = RemoveMeaninglessSpaces(Console.ReadLine() ?? "");
				}
				else
				{
					command = "add";
					if (imageFilepath == "clipboard:" || imageFilepath == "cb:")
					{
						preloadedImage = MySim.ImportImageFromClipboard();
					}
				}
				switch (command)
				{
					case "add":
					NameEntry:
						Console.WriteLine("Enter the name of a new Sim.");
						var newName = Console.ReadLine();
						if (newName == null) goto NameEntry;
					GenderEntry:
						Console.WriteLine("Enter the gender of the Sim. Type 'male' or 'female'.");
						var gender = Console.ReadLine() ?? "";
						try
						{
							ParseGender(gender);
						}
						catch
						{
							goto GenderEntry;
						}
					SignEntry:
						Console.WriteLine("Enter the zodiac sign of the Sim. Type 'aquarius', 'aries', 'cancer', 'capricorn', 'gemini', 'leo', 'libra', 'pisces', 'sagittarius', 'scorpio', 'taurus', or 'virgo'.");
						var sign = Console.ReadLine() ?? "";
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
							AddMySim(new string[] { newName, gender, sign, imageFilepath }, preloadedImage: preloadedImage);
							Console.WriteLine("The operation was completed successfully.");
							break;
						}
						Console.WriteLine("Enter the path of a image of the Sim. Type 'clipboard:' or 'cb:' to read image from clipboard.");
						var filepath = Console.ReadLine() ?? "";
						if (filepath == "") goto ImageFileEntry;
						AddMySim(new string[] { newName, gender, sign, filepath });
						Console.WriteLine("The operation was completed successfully.");
						break;
					case "remove":
						var defaultColor = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Red;
						var c = ShowMySims();
						Console.ForegroundColor = defaultColor;
						if (c == 0) return;
						Console.WriteLine("--------------------------------------------------");
						int i;
						while (true)
						{
							Console.WriteLine("Type the index number of a Sim you want to remove.");
							var number = Console.ReadLine() ?? "";
							if (number != "")
							{
								if (int.TryParse(number, out i))
								{
									break;
								}
							}
						}
						MySimFile.Remove(i);
						Console.WriteLine("The operation was completed successfully.");
						break;
					case "reorder":
						var count = ShowMySims(true);
						if (count == 0) return;
						Console.WriteLine("--------------------------------------------------");
						int source, destination;
					SourceEntry:
						Console.WriteLine("Type the index number of a Sim you want to move.");
						try
						{
							source = int.Parse(Console.ReadLine() ?? "");
						}
						catch
						{
							goto SourceEntry;
						}
					DestinationEntry:
						Console.WriteLine("Type a destination index number.");
						try
						{
							destination = int.Parse(Console.ReadLine() ?? "");
						}
						catch
						{
							goto DestinationEntry;
						}
						MySimFile.Reorder(source, destination);
						Console.WriteLine("The operation was completed successfully.");
						break;
					case "update-image":
						var co = ShowMySims(false);
						if (co == 0) return;
						Console.WriteLine("--------------------------------------------------");
						int index;
					IndexEntry:
						Console.WriteLine("Type the index number of a Sim you want to update its image.");
						try
						{
							index = int.Parse(Console.ReadLine() ?? "");
						}
						catch
						{
							goto IndexEntry;
						}
					ImageEntry:
						Console.WriteLine("Enter the path of a new image of the Sim. Type 'clipboard:' or 'cb:' to read image from clipboard.");
						var fp = Console.ReadLine() ?? "";
						if (fp == "") goto ImageEntry;
						MySimFile.UpdateImage(index, fp);
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
			Console.WriteLine(
$@"================================================================
                SimCity 4 My Sim Tool ({Assembly.GetExecutingAssembly().GetName().Version})
================================================================

  A mod tool for SimCity 4 to add My Sims with custom images

  Commands
  --------
  (empty):
    Prompt inputs in standard input (interactive mode).

  <image_path>:
    Add a new Sim to the list using the image.
    Prompt remaining inputs in standard input (interactive mode).
    This is the same as drag and drop. See the description below.

  help, -h, --help:
    Show this help.

  show [-r, --reorder]:
    Show the My Sim list.
    Show indexes used for reordering when '-r' or '--reorder' is given.

  add <name> <gender> <sign> <image_path>:
    Add a new Sim to the list.
    <gender> = female | male
    <sign> = aquarius | aries | cancer | capricorn | gemini | leo |
             libra | pisces | sagittarius | scorpio | taurus | virgo

  remove <index>:
    Remove the Sim at <index> from the list and delete its image file.
    Use 'show' command to see indexes.

  reorder <source_index> <destination_index>:
    Move the Sim at <source_index> to <destination_index>.
    Use 'show -r' or 'show --reorder' to see indexes.

  update-image <index> <image_path>:
    Update the image of the existing Sim at <index>.
    <image_path> is the path of a new image.
    Use 'show' command to see indexes.

  Drag and Drop
  -------------
  Drag and drop an image to the executable to add a new Sim using its image.

  The image format must be JPG, PNG, GIF, EXIF, TIFF, or BMP.
  To preserve an aspect ratio, bear in mind
  that images are resized to 36x41 pixels automatically.

================================================================");
		}

		private static void AddMySim(string[] args, Bitmap preloadedImage = null)
		{
			if (args.Length == 4)
			{
				var name = args[0];
				var gender = ParseGender(args[1]);
				var zodiacSign = ParseZodiacSign(args[2]);
				var path = args[3];
				var sim = preloadedImage == null ? new MySim(name, gender, zodiacSign, path) : new MySim(name, gender, zodiacSign, preloadedImage);
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
				int i;
				try
				{
					i = int.Parse(args[0]);
				}
				catch
				{
					throw new ArgumentException("'remove' command takes an integer.");
				}
				MySimFile.Remove(i);
			}
			else
			{
				throw new ArgumentException("'remove' command takes 1 argument.");
			}
		}

		private static void ReorderMySim(string[] args)
		{
			if (args.Length == 2)
			{
				int from, to;
				try
				{
					from = int.Parse(args[0]);
					to = int.Parse(args[1]);
				}
				catch
				{
					throw new ArgumentException("'reorder' command takes 2 integers.");
				}
				MySimFile.Reorder(from, to);
			}
			else
			{
				throw new ArgumentException("'reorder' command takes 2 arguments.");
			}
		}

		private static void UpdateSimsImage(string[] args)
		{
			if (args.Length == 2)
			{
				int index;
				try
				{
					index = int.Parse(args[0]);
				}
				catch
				{
					throw new ArgumentException("The index must be an integer.");
				}
				MySimFile.UpdateImage(index, args[1]);
			}
			else
			{
				throw new ArgumentException("'update-image' command takes 2 arguments.");
			}
		}

		private static int ShowMySims(bool reorder = false)
		{
			return MySimFile.Show(reorder);
		}

		private static Gender ParseGender(string gender)
		{
			gender = RemoveMeaninglessSpaces(gender);
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
			zodiacSign = RemoveMeaninglessSpaces(zodiacSign);
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

		private static string RemoveMeaninglessSpaces(string str)
		{
			bool isSpace(char c) => c == ' ';
			var removed = str.ToCharArray().SkipWhile(isSpace).Reverse().SkipWhile(isSpace).Reverse().ToArray();
			return new string(removed);
		}
	}
}
