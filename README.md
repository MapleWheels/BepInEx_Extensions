*Note: This README is still incomplete and this project is in it's usable beta state. It should be almost entirely bug-free on the master branch. Configuration/Tests will contains the most up-to-date examples for basic/standard usage.*

**Re-Write BRANCH README skeleton!**

**Note: This project is not officially associated with BepInEx.**

This [BepInEx Extension library](https://github.com/BepInEx/BepInEx) is a collection of various helper functionalities. As of now the current ready-to-use functions it includes are:

-  **[Configuration File Model](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/BepInEx_Extensions/Configuration/ConfigFileModel.cs)**: 
	
	> Includes virtual methods for easy customization of each variable in Pre and Post BepInEx.Configuration.ConfigFile.Bind() and for common ConfigFile events.
	
	> Configuration Reload Event virtual method for handling ConfigEntry<> property members that have failed to be bound to their configuration file entry.
	
	> Migration: You can now change which ConfigFile a model is using live, including Migration hooks.


**Sample Usage:** 

```csharp
///TBC
```