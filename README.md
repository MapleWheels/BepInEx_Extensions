**Note: This project is not officially associated with BepInEx.**

This [BepInEx Extension library](https://github.com/BepInEx/BepInEx) is a collection of various helper functionalities. As of now the current ready-to-use functions it includes are:

- **Configuration File Model**
	
	> Simple, central place to make your configuration files and instantiate them. An up to date example can be found here: [ExamplePlugin](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/ExamplePlugin.cs) and [ExampleModel](https://github.com/MapleWheels/BepInEx_Extensions/blob/master/ConfigModelTests/Example/ExampleModel.cs)
	
	> Designed to make BepInEx Configuration Files easy to write and instantiate.

	> Includes virtual methods and event hooks for easy customization of each variable in Pre and Post BepInEx.Configuration.ConfigFile.Bind() and for common ConfigFile events.

	> **Migration/Profile Support**: You can now change which ConfigFile a model is using live, including Migration hooks, with a simple, one-line command.
	
	> Allows you to create your own types/classes, just implement the interfaces and the rest will work.


**Sample Usage:** 

```csharp
public class ExampleModel : ConfigDataModel
{
	//Notes: 
	//You do not need to do every single different way here, it's just to show you the different ways it can be used. 
	//Pick the way that makes sense to you and just use that one.

	public ConfigData<float> ConfigOption1 = new ConfigData<float>()    //SIMPLE/Minimalist setup, using a field.
	{
		DefaultValue = 100f,
		DescriptionString = "Hello"
	};

	public ConfigData<float> ConfigOption2 { get; set; } = new ConfigData<float>()  
	{
		Key = "Config_Variable_Name",                                   //OPTIONAL: This will be set to 'Config_Variable_Name' in the config file. If not set by you, the default 'Key' name is the variable's name.
		DefaultValue = 10f,                                             //REQUIRED: Default value.
		DescriptionString = "I'm running out of flavor text",           //OPTIONAL: Description.
		AcceptableValues = new AcceptableValueRange<float>(0f, 50f)     //OPTIONAL: Acceptable values.
	};

	public ConfigData<int> ConfigOption3;       //Nothing defined. This will be bound with the Type defaults.


	public override void SetDefaults()
	{
		this.SectionName = "Example Section";   //Define your section name here. 
	}
}

[BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
public class ExamplePlugin : BaseUnityPlugin
{
	//NOTE: Before you read this, please take a look at the ExampleModel.cs file. The below will make a lot more sense if you do.
	ExampleModel model;

	void Awake()
	{
		//NOTE: Before you read this, please take a look at the ExampleModel.cs file. The below will make a lot more sense if you do.
		model = Config.BindModel<ExampleModel>(Logger); //Initialized and ready to use.

		Logger.LogInfo($"ExamplePlugin: model init completed.");
		Logger.LogInfo($"ExamplePlugin: model.ConfigOption1={ model.ConfigOption1.Value }");

		model.ConfigOption2.Value = 20f;                                                              
		Logger.LogInfo($"ExamplePlugin: model.ConfigOption2={ (float) model.ConfigOption2 }");    //Explicit & implicit conversion is supported.

		Logger.LogInfo($"ExamplePlugin: model.ConfigOption3={ (int) model.ConfigOption3 }");    //All defaults. Value = 0.

		//Want to change config files for profile support?
		ConfigFile profile2 = new ConfigFile(System.IO.Path.Combine(Paths.BepInExConfigPath, "ExamplePlugin", "profile2"), true);
		model.SetConfigFile(profile2);
	}
}
```