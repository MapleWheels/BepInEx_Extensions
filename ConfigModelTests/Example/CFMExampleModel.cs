using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigModelTests.Example
{
    /// <summary>
    /// This is an example config file model.
    /// </summary>
    [ConfigModelSectionName(Value = "General")] //How you define the section name. 'Default' will be used if this is not set/is missing.
    public class CFMExampleModel : ConfigFileModel
    {
        [ConfigEntryDefaultValue(Value = 10)]   //Default value
        [ConfigEntryDescription(Value = "Here is where you put the description.")]
        public ConfigEntry<int> ConfigVariable1 { get; set; }

        [ConfigEntryDefaultValue(Value = 10f)]  //Make sure you get the data type right here, for example 10f instead of 10 for float.
        [ConfigEntryDescription(Value = "Here is where you put the description.")]
        public ConfigEntry<float> ConfigVariable2 { get; set; }

        [ConfigEntryDefaultValue(Value = 10d)]
        [ConfigEntryDescription(Value = "Here is where you put the description.")]
        [ConfigEntryKey(Value = "ThisIsACustomNameForTheConfigFile")]   //OPTIONAL: You can change the name that this variable is bound to in the config file.
        public ConfigEntry<decimal> ConfigVariable3 { get; set; }


        //Want to change a value dynamically at run time? Use one of the virtual methods! See ConfigFileModel for a full list.
        protected override void PrePropertyBind<T>(PropertyInfo property, ref string sectionName, ref string key, ref T defaultValue, ConfigDescription description, ref ConfigFile file, ref bool useStandardBindingMethod)
        {
            //This method is called on every single ConfigEntry right before it is bound. Change whatever you wish here.
            if (key == "ConfigVariable2")
            {
                //Lets change it to something else!
                key = "SoySauce";
            }
        }
    }
}
