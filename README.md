*Note: This README is still incomplete and this project is in it's usable beta state. It should be almost entirely bug-free on the master branch. Configuration/Tests will contains the most up-to-date examples for basic/standard usage.*

**Note: This project is not officially associated with BepInEx.**

This [BepInEx Extension library](https://github.com/BepInEx/BepInEx) is a collection of various helper functionalities. As of now the current ready-to-use functions it includes are:

-  **[Configuration File Model](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/BepInEx_Extensions/Configuration/ConfigFileModel.cs)**: 

	> This was developed in the spirit of *Entity Framework Core*'s Data-Model class.
	
	> Includes virtual methods for easy customization of each variable in Pre and Post BepInEx.Configuration.ConfigFile.Bind() and for common ConfigFile events.
	
	> Allows you to develop an attribute-style configuration file model. **See [CFMExampleModel.cs](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/CFMExampleModel.cs)** for an example model and **See [CFMExamplePlugin.cs](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/CFMExamplePlugin.cs)** for how it's used.
	
	> Configuration Reload Event virtual method for handling ConfigEntry<> property members that have failed to be bound to their configuration file entry.
	
	> Migration: You can now change which ConfigFile a model is using live, including Migration hooks.


**Sample Usage:** 

```csharp
[
public CFMExampleModel : ConfigFileModel
{
	[ConfigEntryDefaultValue(Value = 10)]   //Default value
    [ConfigEntryDescription(Value = "Here is where you put the description.")]
    [ConfigEntryKey(Value = "ThisIsACustomNameForTheConfigFile")] //OPTIONAL: You can change the name that this variable is bound to in the config file.
    public ConfigEntry<int> ConfigVariable1 { get; set; }
}


[BepInPlugin("com.example.configModelExample", "Config Model Example", "0.0.0")]
public class CFMExamplePlugin : BaseUnityPlugin
{
	void Awake()
	{
		//First instantiate and bind your config file.
		CFMExampleModel configFileInstance = Config.BindModel<CFMExampleModel>();

		//Now use it, it's ready to go!
		Logger.LogInfo($"Hey, the CFM Example Model works!");
		Logger.LogInfo($"ConfigVariable1 = {configFileInstance.ConfigVariable1.Value}");
		
		//Want to change the active config file (for example, to support profiles)? Rebind it.
		ConfigFile newConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "custom_config.cfg"), true);
		configFileInstance.ChangeConfigFile(newConfigFile);
	}
}
```