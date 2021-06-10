<img src="docs/banner.png">  

# Going Medieval - Mod Launcher
This project allows the development and loading of mods for Going Medieval, written in C#.

The currently supported version is 0.5.28.6.

## Features
The following features are supported by the Mod Launcher:
- load C# based mods using a simple Plugin-API
- access the native unity API / runtime
- replace / patch methods by using [Harmony](https://harmony.pardeike.net/)
- No manual editing / hacking of files - the games files don't get modified 
- Logging
 - ModLauncher / Plugin related logging within the 'mod_launcher.log' file
 - Unity related logging within the 'output_log' file

### Mods
The following mods are realized by using the Plugin-API\

[BugReportDisablerPlugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/BugReportDisablerPlugin)\
Disables the bug-reporting feature & ui so there are no error-popups while experimenting

[CameraSettingsPlusPlus](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/CameraSettingsPlusPlus)\
Allows (almost) unlimited zoom for the user-camera

[DeveloperConsoleEnablerPlugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/DeveloperConsoleEnablerPlugin)\
Enables the Developer-Console by using L to open and K to close the UI 

## Installation:
Download the latest release and drop it into your going medieval folder

## Technical

### Developing a custom mod / plugin
_Note: The terms mod, plugin and assembly are used interchangeably - in the context of this project they mean the same thing._

TODO :)

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
- Unity-Doorstop for injecting code: https://github.com/NeighTools/UnityDoorstop
- Harmony for runtime patching of code: https://harmony.pardeike.net/


