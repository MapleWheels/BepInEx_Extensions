
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;

namespace ConfigModelTests.Example
{
    /// <summary>
    /// And example for using the ConfigDataModel and ConfigData types. 
    /// </summary>
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
        }.PreBindSubscribe((ConfigFile file, ManualLogSource logger) =>
        {
            //OPTIONAL: You can run anything you need to here BEFORE ConfigFile.Bind() is called.
        }).PostBindSubscribe((ConfigFile file, ManualLogSource logger) =>
        {
            //OPTIONAL: You can run anything you need to here AFTER ConfigFile.Bind() is called.
        });

        public ConfigData<int> ConfigOption3;       //Nothing defined. This will be bound with the Type defaults.


        public override void SetDefaults()
        {
            this.SectionName = "Example Section";   //Define your section name here. 
        }
    }
}
