*Note: This README is still very much incomplete and this project is in it's usable alpha state, although it should be mostly bug-free on the master branch. Configuration/Tests will contains the most up-to-date examples for basic/standard usage.*

This [BepInEx Extension library](https://github.com/BepInEx/BepInEx) is a collection of various helper functionalities. As of now the current ready-to-use functions it includes are:
-  **[Configuration File Model](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/BepInEx_Extensions/Configuration/ConfigFileModel.cs)**: 

	> This was developed in the spirit of *Entity Framework Core*'s Data-Model class.
	
	> Includes virtual methods for easy customization of each variable in Pre and Post BepInEx.Configuration.ConfigFile.Bind() and for common ConfigFile events.
	
	> Allows you to develop an attribute-style configuration file model. **See [ConfigModelTests.cs](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Tests/ConfigModelTestModel.cs)** for an example.
	
	> **New**: Configuration Reload Event virtual method for handling ConfigEntry<> property members that have failed to be bound to their configuration file entry.
	
	> **New**: Migration: You can now change which ConfigFile a model is using live, including Migration hooks.
	

Coming Soon:
- Entity Framework's DbContext-style model for wrapping all model's based on their alloted ConfigFile.