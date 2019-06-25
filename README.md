# SimCity 4 My Sim Tool

A mod tool for SimCity 4 to add your original My Sims with custom images

[Download the latest version](https://github.com/curegit/sc4-my-sim-tool/releases/)

![My Sim View](Screenshots/MySims.png)

## Requirements

- Windows
- .NET Framework 4.0+
- SimCity 4 Rush Hour/Deluxe

## Installation



## Usage

### Commands

`help, -h, --help`  
Show the help.

`show [-r, --reorder]`  
Show the list of My Sims written in the DAT file.  
When '-r' or '--reorder' is given, indexes used for reordering are also shown.

`add <name> <gender> <sign> <image_path>`  
Add a new Sim to the list.  
<image_path> is absolute or relative.  
The image format must be JPG, PNG, GIF, EXIF, TIFF, or BMP.

`remove <index>`  
Remove a specified Sim from the list and delete its image file.  
Use 'show' command to see indexes.

`reorder <source_index> <destination_index>`  
Reorder a specified Sim.  
Move the Sim to a given index position.  
Use 'show -r' to see indexes.

  Commands
  --------
  (empty):
    Prompt inputs in standard input (interactive mode).

  <image_path>:
    Add a new Sim to the list using the image.
    Prompt remaining inputs in standard input (interactive mode).
    This is same as drag and drop. See the description below.

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

### Interactive Mode

If you execute the application with no command line arguments, it prompts inputs in standard input.

### Drag and Drop

Drag and drop an image file to the executable to add a new Sim using its image.

![Lenna DD Example](Screenshots/Lenna.gif)

### Gender Values

Type one of these to input gender.

- female
- male

### Sign Values

Type one of these to input sign.

- aquarius
- aries
- cancer
- capricorn
- gemini
- leo
- libra
- pisces
- sagittarius
- scorpio
- taurus
- virgo

## Notes

- The DAT file and images will be saved in `{MyDocuments}/SimCity 4/MySim`.
- Information of Sims will be copied into each save of cities where the Sim lives except the bitmap.
- If you remove a Sim from the DAT file, the Sim remains in cities, but the image will be missing.

## Gallery

![Miku Lives](Screenshots/Living.png)

## License

[The Unlicense](LICENSE)
