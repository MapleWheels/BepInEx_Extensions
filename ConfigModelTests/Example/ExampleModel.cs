using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx.Extensions.Configuration;
using BepInEx.Configuration;

namespace ConfigModelTests.Example
{
    /// <summary>
    /// And example for using the ConfigDataModel and ConfigData types. 
    /// </summary>
    public class ExampleModel : ConfigDataModel
    {
        //Notes: 
        //You don't need to do every single different way here, it's just to show you the different ways it can be used. 
        //Pick the way that makes sense to you and just use that one.

        public ConfigData<float> ConfigOption1 { get; set; } //must be a property.
        public ConfigData<float> ConfigOption2 { get; set; } = new ConfigData<float>()  //Option A: constructor instantiation style. You can do this or the SetDefaults style below. Your preference.
        {
            Key = "Config_Variable_Name",   //This will be set to 'Config_Variable_Name' in the config file. If not set by you, the default 'Key' name is the variable's name.
            DefaultValue = 10f,             //Default value.
            DescriptionString = "I'm running out of flavor text",       //Description
            AcceptableValues = new AcceptableValueRange<float>(0f, 50f)     //Acceptable values
        }.PreBindSubscribe((ConfigFile file) =>
        {
            //OPTIONAL: You can run anything you need to here BEFORE ConfigFile.Bind() is called.
        }).PostBindSubscribe((ConfigFile file) =>
        {
            //OPTIONAL: You can run anything you need to here AFTER ConfigFile.Bind() is called.
        });

        public ConfigData<int> ConfigOption3 { get; set; }  //Left uninitialized for ExamplePlugin example.

        public override void SetDefaults()
        {
            //Note: This is a helper virtual method that will be called when the model is first instantiated.
            //You can also do some initialization here, it's up to you if you prefer using the above method or this one.

            this.SectionName = "Example Section";   //Define your section name here. 

            //you don't need to define everything here; just DefaultValue and DescriptionString are recommended.
            this.ConfigOption1 = new ConfigData<float>()    //SetDefaults instantiation style.
            {
                DefaultValue = 15f,
                DescriptionString = "Here's the description for the config file.",
                AcceptableValues = new AcceptableValueList<float>(5f, 10f, 15f, 20f),
                Tags = new object[] { "A", "B", "C", "iRanOutOfIdeas" }
            };

            //Want to do something before/after the value is bound? Just hook into an event.
            ConfigOption1.PreBind += (ConfigFile ConfigToBeBoundTo) =>
            {
                ConfigOption1.DefaultValue = 18f;
                Logger?.LogInfo("ConfigOption1 is about to be bound.");
            };

            ConfigOption1.PostBind += (ConfigFile ConfigToBeBoundTo) =>
            {
                ConfigOption1.DefaultValue = 18f;
                Logger?.LogInfo("ConfigOption1 is about to be bound.");
            };
        }
    }
}
