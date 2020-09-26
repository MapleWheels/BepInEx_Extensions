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
        public ConfigData<float> ConfigOption1 { get; set; } //must be a property.
        public ConfigData<float> ConfigOption2 { get; set; } = new ConfigData<float>()  //constructor instantiation style. You can do this or the SetDefaults style below. Your preference.
        {
            Key = "Config_Variable_Name",   //This will be set to 'configOption2' if not set by you. Defaults to the variable name.
            DefaultValue = 10f,
            DescriptionString = "I'm running out of flavor text",
            AcceptableValues = new AcceptableValueRange<float>(0f, 50f)
        };
        public ConfigData<int> ConfigOption3 { get; set; }  //Left uninitialized for ExamplePlugin example.

        public override void SetDefaults()
        {
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
