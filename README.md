# Mod Loader for The Long Dark

This mod loader allows users to easily install and run lots of mods at once!  
It also makes the modder's job easier by making method patching a breeze.  
Furthermore, the mod DLLs should only include the code that is being changed instead of the whole assembly.  
Thus, mods are generally only a few kilobytes in size and usually don't contain any copyrighted material.

*This project uses [Harmony](https://github.com/pardeike/Harmony) for its method patching.
Many thanks to the developers of this amazing tool!*

## Compatibility

This mod loader has been tested for **The Long Dark v1.56** on **Windows x64** and **Linux x64**.  
**Mac OS** and 32-bit operating systems have not been tested, but the tool should theoretically work there, too.  
If you are able to test the the installer and mod loader on any of these platforms,
please report your findings - whether it be successful or not - to the
[issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues). Thank you!

## Installation instructions

### **Note that only TLD versions up to 1.56 are supported!**

#### Windows (64-bit):

- **Download** `ModLoaderInstaller-Windows-x64.zip` from the [downloads page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases/)
- **Extract** the entire ZIP archive
- **Run** `ModLoaderInstaller.exe` by double-clicking the file.
- Select TLD's **executable file** (e.g. at `C:\Program Files (x86)\Steam\SteamApps\common\TheLongDark\tld.exe`)
- Check that the "file status" label now says `Valid, unpatched` or `Valid, outdated`
- Press the "Patch" or "Update" button
- If the text in the lower left corner says "Done!", the mod loader has been installed correctly.  
  If it doesn't, please create an issue on the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues)!

#### MacOS:

- **Download** `ModLoaderInstaller-Mac.zip` from the [downloads page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases/)
- **Extract** the entire ZIP archive
- **Run** `ModLoaderInstaller-Mac` by double-clicking the file.
- Select TLD's **executable file** (usually at `~/Library/Application Support/Steam/steamapps/common/TheLongDark/tld.app`)
- Check that the "file status" label now says `Valid, unpatched` or `Valid, outdated`
- Press the "Patch" or "Update" button
- If the text in the lower left corner says "Done!", the mod loader has been installed correctly.  
  If it doesn't, please create an issue on the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues)!

#### Linux (64-bit):

- **Download** `ModLoaderInstaller-Linux-x64.tar.gz` from the [downloads page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases/latest/)
- Open a terminal in your downloads folder and run the following command:
```
tar xf ModLoaderInstaller-Linux-x64.tar.gz && ./ModLoaderInstaller/ModLoaderInstaller
```
- Select TLD's **executable file** (usually at `~/.steam/steam/steamapps/common/TheLongDark/tld.x86`)
- Check that the "file status" label now says `Valid, unpatched` or `Valid, outdated`
- Press the "Patch" or "Update" button
- If the text in the lower left corner says "Done!", the mod loader has been installed correctly.  
  If it doesn't, please create an issue on the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues)!

#### 32-bit operating systems / other

- **Download** `ModLoaderInstaller.jar` from the [downloads page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases/latest/)
- Make sure a Java 8 runtime that includes JavaFX is installed (e.g. [Amazon Corretto 8](https://docs.aws.amazon.com/corretto/latest/corretto-8-ug/downloads-list.html))
- Run `java -jar ModLoaderInstaller.jar`
- Select TLD's **executable file**
- Check that the "file status" label now says `Valid, unpatched` or `Valid, outdated`
- Press the "Patch" or "Update" button
- If the text in the lower left corner says "Done!", the mod loader has been installed correctly.  
  If it doesn't, please create an issue on the [issues page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/issues)!

### Installing mods:

- It is **strongly recommended** that you use [WulfMarius's Mod-Installer](https://github.com/WulfMarius/Mod-Installer/releases) to automatically install and manage your mods.

Manual installation: *(Not recommended)*
- Add the mod's DLL file(s) to the directory called  
  `<Steam>/SteamApps/common/TheLongDark/mods`,  
  which the installer program should've already created.

### Troubleshooting:

- If the installer doesn't let you patch `Rewired_Core.dll` because it says the file is `Invalid`,  
  you'll need to restore the original DLL file:
  - If you've installed an earlier version of the mod loader, re-run the old installer to unpatch the file.
  - Alternatively, if you're using Steam, you can use the [Verify integrity of game files](https://support.steampowered.com/kb_article.php?ref=2037-QEUH-3335) option.
  - If you've pirated TLD, I can't help you. You'll need to buy the game or at least find a distribution that includes binary-identical versions of all files.

### For developers:

- Install the mod loader as usual.
- In Visual Studio, create a new **.NET Framework Class Library** project
- Open the project settings and set the compilation target to **.NET Framework 3.5** or lower
- Add the following files from `<Steam>/SteamApps/common/TheLongDark/tld_Data/Managed/` as dependencies:
  - `Assembly-CSharp.dll`
  - `0Harmony.dll`
  - `UnityEngine.CoreModule.dll`
  - You may also need other UnityEngine DLLs in some occasions
- Follow the instructions in the [Harmony wiki](https://github.com/pardeike/Harmony/wiki) on how to patch existing methods
- The mod loader will also call any `public static void OnLoad()` methods in any types it can find.

## Mod List

[Moved to this wiki page](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/wiki/Mod-List)
