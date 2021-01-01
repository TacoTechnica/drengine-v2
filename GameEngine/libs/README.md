# LIBRARY TUTORIAL

Most likely if you try compiling this program from scratch you'll be missing some libraries. Here's how to get them set up:

# MONOGAME

Install monogame. Run the "Getting Started" program to make sure it works. TODO: Add link (for now just look it up)

# NUGET LIBRARIES

Get Nuget working and install the following libraries in the project:
- GtkSharp
- NDesk.Options
- SpriteFontPlus
- Newtonsoft JSON
- BassMixer
- TextCopy

# BASS MIXER

1) Download the BASS Audio library from http://www.un4seen.com/ (click on the left side horizontal pane -> BASS). Download for your specific platform and find bass.dll (or libbass.so) located in the x64 directory.
2) Move both bass.dll/libbass.so files here (in the libs folder)
3) That's it! If the program compiles correctly or fails to run, the dll/so file might not copy properly to the output directory.

# HELP!

Do ALL of these apply?
1) You tried everything and things are still broken
2) You tried searching for answers online and tried getting a grasp of what is going on first/what's actually wrong (not just what the error message is!)
3) You currently don't know how/cant fix the problem

If so, don't hesitate to shoot me a message! I'm totally down to help you out! Try to include as much info as possible (screenshots, logs, steps you followed and where things went wrong etc.)

