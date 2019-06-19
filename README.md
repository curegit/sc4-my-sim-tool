# SC4 My Sim Tool

A mod tool for SimCity 4

You can add your original My Sims with custom images.

[Download the latest version](https://github.com/curegit/sc4-my-sim-tool/releases/)

![My Sim View](Screenshots/MySims.png)

## Requirements

- Windows
- .NET Framework 4.0+
- SimCity 4 Rush Hour/Deluxe

## Usage

### Interactive

If you execute the application with no command line arguments, it prompts inputs in standard input.

![Abe Stdin Example](Screenshots/Abe.png)

### Commands

`help`  
Show the help.

`show [-i]`  
Show the list of My Sims written in the DAT file.  
When '-i' is given, indexes used for reordering are also shown.

`add <name> <gender> <sign> <image_path>`  
Add a new Sim to the list.  
<image_path> is absolute or relative.  
The image format must be JPG, PNG, GIF, EXIF, TIFF or BMP.

`remove <name>`  
Remove a specified Sim from the list and delete its image file.

`reorder <source_index> <destination_index>`  
Reorder a specified Sim.  
Move him/her to a given index position.  
Use 'show -i' to see indexes.

### Drag and Drop

Drag and drop a image file to the exe to add a new Sim using its image.

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

- The DAT file and images will be saved in `{MyDocuments}/SimCity 4/MySim`
- Information of Sims will be copied into each save of cities where the Sim lives except the bitmap.
- If you remove a Sim from the DAT file, the Sim remains in cities, but the image will be missing.

![Miku Lives](Screenshots/Living.png)
