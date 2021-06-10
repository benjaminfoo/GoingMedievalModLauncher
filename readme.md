<img src="docs/banner.png">  

# Going Medieval - Mod Launcher
This project allows the development and loading of mods for Going Medieval, written in C#.

The currently supported version is 0.5.28.6.

## Features
The following features are supported by the Mod Launcher:
- load C# based mods using a simple [Plugin-API](https://github.com/benjaminfoo/GoingMedievalModLauncher/blob/main/GoingMedievalModLauncher/src/plugins/IPlugin.cs)
- access the native unity API / runtime
- replace / patch methods by using [Harmony](https://harmony.pardeike.net/)
- No manual editing / hacking of files - the games files don't get modified 
- Logging
 - ModLauncher / Plugin related logging within the 'mod_launcher.log' file
 - Unity related logging within the 'output_log' file

### Mods
The following mods are realized by using the Plugin-API\

[Additional Map Sizes - Plugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/AdditionalMapSizesPlugin)\
A simple mod which adds different map sizes to the new game - map size dropdown.

[BugReport Disabler - Plugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/BugReportDisablerPlugin)\
Disables the bug-reporting feature & ui so there are no error-popups while experimenting.

[CameraSettings++ - Plugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/CameraSettingsPlusPlus)\
Increases rendering-, zoom- & shadow-distance - may slow down performance.

[DeveloperConsole Enabler - Plugin](https://github.com/benjaminfoo/GoingMedievalModLauncher/tree/main/DeveloperConsoleEnablerPlugin)\
Enables the Developer-Console by using L to open and K to close the UI.

## Installation:
Download the latest release and drop it into your going medieval folder

## Technical

For technical information checkout the [wiki](https://github.com/benjaminfoo/GoingMedievalModLauncher/wiki/Technical) at the github-page. 

## Additional
- Unity-Doorstop for injecting code: https://github.com/NeighTools/UnityDoorstop
- Harmony for runtime patching of code: https://harmony.pardeike.net/


