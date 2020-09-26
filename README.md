**Note: This project is not officially associated with BepInEx.**

This [BepInEx Extension library](https://github.com/BepInEx/BepInEx) is a collection of various helper functionalities. As of now the current ready-to-use functions it includes are:

- **Configuration File Model**
	
	> Simple, central place to make your configuration files and instantiate them. An up to date example can be found here: [ExamplePlugin](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/ExamplePlugin.cs) and [ExampleModel](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/ExampleModel.cs)
	
	> Supports events for migrations/config file changes.
	
	> Supports pre-bind and post-bind events for config entries.
	
	> Allows you to create your own types/classes, just implement the interfaces and the rest will work.


**Sample Usage:** 

```csharp
public class ExampleModel : ConfigDataModel
{
	public ConfigData<float> ConfigOption1 { get; set; } //must be a property.
	public ConfigData<float> ConfigOption2 { get; set; } = new ConfigData<float>()  //constructor instantiation style.
	{
		Key = "Config_Variable_Name",   //This will be set to 'configOption2' if not set by you. Defaults to the variable name.
		DefaultValue = 10f,
		DescriptionString = "I'm running out of flavor text",
		AcceptableValues = new AcceptableValueRange<float>(0f, 50f)
	};
	public ConfigData<float> ConfigOption3 { get; set; } //Intentionally left un-initiated. Late bind style.

	public override void SetDefaults()
	{
		this.SectionName = "Example Section";   //Define your section name here. 

		//you don't need to define everything here;  DefaultValue and DescriptionString are recommended.
		this.ConfigOption1 = new ConfigData<float>()    //SetDefaults instantiation style.
		{
			DefaultValue = 15f,
			DescriptionString = "Here's the description for the config file."
		};
	}
}

[BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
public class ExamplePlugin : BaseUnityPlugin
{
	ExampleModel model;

	void Awake()
	{
		model = Config.BindModel<ExampleModel>(Logger); //Initialized and ready to use.
		Logger.LogInfo($"ExamplePlugin: model.ConfigOption1={model.ConfigOption1.Value}");

		model.ConfigOption1.Value = 20f;
		Logger.LogInfo($"ExamplePlugin: model.ConfigOption1={model.ConfigOption1.Value}");

		//If you didn't initialize an entry in your config model type, or you want to do it externally, you can do so here. 
		//Late bind style.
		model.ConfigOption3 = new ConfigData<int>()
		{
			DefaultValue = 10,
			DescriptionString = "hello",
			SectionName = model.SectionName,
		}.Bind(Config, Logger); //Late Bind Call

		//Want to change config files for profile support? Easy.
		ConfigFile profile2 = new ConfigFile(
			System.IO.Path.Combine(Paths.BepInExConfigPath, "ExamplePlugin", "profile2"), true);	//Profile config file.
		model.SetConfigFile(profile2);	//New profile active.
	}
}
```