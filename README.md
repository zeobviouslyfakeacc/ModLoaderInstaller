# Mod Loader for The Long Dark

Only being able to install one mod at a time sucks and having to ship the entirety of `Assembly-CSharp.dll` for a mod that changes a few lines of code isn't great, either.

**Enter TLD Mod Loader**

This mod loader allows users to easily install and run lots of mods at once!  
It also makes the modder's job easier by making method patching a breeze.  
Furthermore, the mod DLLs should only include the code that is being changed instead of the whole assembly.  
Thus, mods are generally only a few kilobytes in size and usually don't contain any copyrighted material.

*This project uses [Harmony](https://github.com/pardeike/Harmony) for its method patching. Many thanks to the developers of this amazing tool!*

## Compatibility

This tool has been tested for **The Long Dark v0.426** on **Windows x64** and **Linux x64**.  
Mac OS and 32-bit operating systems have not been tested, but the tool should theoretically work there, too.  
If you are able to test the the installer and mod loader on any of these platforms, please report your findings - whether it be successful or not - to the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues). Thank you!

## Installation instructions

### For users

**Installing the tool:**

- **Download** the latest release from the [downloads page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases)
  - This is a JavaFX 8 application. Make sure you have Java 8 installed.  
    **Linux users** might have to install the `openjfx` package if they're using the OpenJDK distribution
- **Run** the Java application  
  **Windows users** can just double click the JAR file  
  **Linux users** may need to use `java -jar ModLoaderInstaller.jar`
- Select the file `<Steam>/SteamApps/common/TheLongDark/tld_data/Managed/Rewired_Core.dll`
- Press the "Patch" or "Update" button
- If the text in the lower left corner says "Done!", the mod loader has been installed correctly.  
  If it doesn't, please create an issue on the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues)!

**Installing mods:**

- Add the mod's DLL file(s) to the directory called  
  `<Steam>/SteamApps/common/TheLongDark/mods`,  
  which the installer program should've already created.

### For developers:

- Install the mod loader as usual.
- In Visual Studio, create a new **.NET Framework Class Library** project
- Open the project settings and set the compilation target to **.NET Framework 3.5** or lower
- Add the following files from `<Steam>/SteamApps/common/TheLongDark/tld_data/Managed/` as dependencies:
  - `Assembly-CSharp.dll`
  - `Harmony.dll`
  - `UnityEngine.dll`
- Follow the instructions in the [Harmony wiki](https://github.com/pardeike/Harmony/wiki) on how to patch existing methods
- The mod loader will also call any `public static void OnLoad()` methods in any types it can find.

## Mod List

- [AddModdedToVersionString](https://github.com/zeobviouslyfakeacc/MiniMods/tree/master/AddModdedToVersionString) by zeobviouslyfakeacc (installed by default)
- [DeveloperConsole](https://github.com/FINDarkside/TLD-Developer-Console) by FINDarkside
- [DisableTracking](https://github.com/zeobviouslyfakeacc/MiniMods/tree/master/DisableTracking) by zeobviouslyfakeacc
- [RememberBreakDownItem](https://github.com/zeobviouslyfakeacc/MiniMods/tree/master/RememberBreakDownItem) by zeobviouslyfakeacc
- If you've created a mod and would like it to be included in this list, please contact me! :)
