# LIBRARY TUTORIAL

Most likely if you try compiling this program from scratch you'll be missing some libraries. Here's how to get them set up:

# MONOGAME

Install monogame. Run the "Getting Started" program to make sure it works. TODO: Add link (for now just look it up)

# NUGET LIBRARIES

Get Nuget working and install the following libraries in the project:
	- GtkSharp
	- NDesk.Options
	- SpriteFontPlus
	- YamlDotNet

# BASS MIXER

This is the cross platform audio library DREngine uses.

WARNING: Sadly Nuget will not work here, as you must

Here's the installation steps:

1) Clone/Download the repo from https://github.com/ManagedBass/ManagedBass/
2) Open up ManagedBass/src/ManagedBass.sln in Visual Studio or JetBrains Rider
3) Build the following projects:
	- src/Bass/Portable/Bass.csproj
	- src/AddOns/BassMix/BassMix.csproj
4) Find where the .dll files for the project were built (for me the two DLLs I needed were in src/Bass/Portable/bin/Debug/netstandard1.4/ManagedBass.dll and src/AddOns/BassMix/bin/Debug/netstandard1.4/ManagedBass.Mix.dll)
5) Add the .dll files as a reference in the project (look up how to do it for your IDE). Now there should be no compiler warnings. If so, good stuff! However, you're not done yet.
6) Download the BASS Audio library from http://www.un4seen.com/ (click on the left side horizontal pane -> BASS). Download for your specific platform and find bass.dll located in the x64 directory.
7) Download the BASSMix Audio library (from the same page as the BASS audio library). Find bassmix.dll located in x64
8) Copy both bass.dll and bassmix.dll here (in the libs folder)
9) Now when you compile & run the program, it should work. If your program compiles but has ManagedBass library issues, you probably used the wrong dlls from steps 6-8. If it fails to compile, something went wrong from steps 2-5.


# HELP!

Do ALL of these apply?
1) You tried everything and things are still broken
2) You tried looking up online for any solutions to your problems and tried getting a grasp of what is going on first before charging along
3) You have no clue how to fix the problem

If so, shoot me a message! I'm totally down to help you out! Try to include as much info as possible (screenshots, logs, steps you followed and where things went wrong etc.)
