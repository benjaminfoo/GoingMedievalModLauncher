# Going Medieval Mod Launcher
This project allows the loading of mods within Going Medieval.

The currently supported version is 0.5.28.6

## Features
- load C# based mods into Going Medieval
- replace / patch methods within the going medieval API
- No manual file editing / hacking - the games files / data don't get touched at all

### Currently integrated mods
- BugReport-Disabler: Disables the bug-reporting so there are no error-popups while experimenting
- DevConsole-Enable: Enables the Developer-Console by using K to open and L to close
- Camera-Settings: Allows (almost) unlimited zoom for the user-camera 

## Installation:
Download the latest release and drop it into your going medieval folder

## Technical

### Procedure
The following procedure is executed when using the mod-loader:
- winhttp.dll is used to inject code during runtime into the unity process
- doorstop_config.ini is a key-value file which controls the initialization of the injection process
- winhttp.dll then launches the code defined in Launcher.cs which contains a Main-Method

### Used libraries
The following libraries are referenced when compiling the mod loader and the contained mods:
- 0Harmony
- Assembly-CSharp
- System
- System.Core
- UnityEngine
- UnityEngine.CoreModule
- UnityEngine.IMGUIModule
- UnityEngine.InputLegacyModule
- UnityEngine.InputModule
- UnityEngine.UIModule

## Additional
- Unity-Doortstop for injecting code: https://github.com/NeighTools/UnityDoorstop
- Harmony for runtime patching of code: https://harmony.pardeike.net/


