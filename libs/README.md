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

WARNING: Sadly Nuget will not work here, as there is a weird library incompatability issue (https://github.com/ManagedBass/ManagedBass/issues/71). You must compile from source. Thankfully it's not too involved.

Here's the installation steps:

1) Clone/Download the repo from https://github.com/ManagedBass/ManagedBass/
2) Open up ManagedBass/src/ManagedBass.sln in Visual Studio or JetBrains Rider
3) Build the following projects:
	- src/Bass/Portable/Bass.csproj
	- src/AddOns/BassMix/BassMix.csproj
4) Find where the .dll files for the project were built (for me the two DLLs I needed were in src/Bass/Portable/bin/Debug/netstandard1.4/ManagedBass.dll and src/AddOns/BassMix/bin/Debug/netstandard1.4/ManagedBass.Mix.dll)
5) (optional) Move ManagedBass.dll and ManagedBass.Mix.dll into the libs folder, so you can delete the cloned repo after you are done without breaking references in the project.
6) Add the .dll files as a reference in the project (look up how to do it for your IDE). Now there should be no compiler warnings. If so, good stuff! However, you're not done yet.
7) Download the BASS Audio library from http://www.un4seen.com/ (click on the left side horizontal pane -> BASS). Download for your specific platform and find bass.dll (or libbass.so) located in the x64 directory.
8) Download the BASSMix Audio library (from the same page as the BASS audio library). Find bassmix.dll (or libbassmix.so) located in x64
9) Move both dll/so files from steps 7 and 8 here (in the libs folder)
10) Now when you compile & run the program, it should work. If your program compiles but has ManagedBass library issues during runtime, you probably used the wrong dlls from steps 6-8. If it fails to compile, something went wrong from steps 2-5.

Note: Linux uses .so for steps 6-8, everything else is the same (and it should work). However, if I try to use libraries normally we encounter the error "BASSMix: BASS must be loaded first" as seen in http://www.un4seen.com/forum/?topic=18656.0.
I could not for the life of me figure out how to fix this, so in Game/Audio/AudioMixer.cs I don't use the library in linux (hence the janky OS dependent code). I should fix this later (but it's not that big of an issue).
Therefore, you technically don't need libbassmix.so if you're on linux.

# HELP!

Do ALL of these apply?
1) You tried everything and things are still broken
2) You tried searching for answers online and tried getting a grasp of what is going on first/what's actually wrong (not just what the error message is!)
3) You currently don't know how/cant fix the problem

If so, don't hesitate to shoot me a message! I'm totally down to help you out! Try to include as much info as possible (screenshots, logs, steps you followed and where things went wrong etc.)

