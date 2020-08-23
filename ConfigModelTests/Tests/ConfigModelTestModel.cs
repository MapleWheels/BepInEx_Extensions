using BepInEx.Configuration;
using System;
using System.Reflection;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;

/// <author> PerfidiousLeaf </author>
/// <date>2020-08-13</date>
namespace ConfigModelTests.Tests
{
    /// <summary>
    /// A ConfigFileModel example use case and model test.
    /// </summary>
    [ConfigModelSectionName(Value = "This_Is_An_Example_Test_Section")] //This gets changed in OnModelCreate()
    public class ConfigModelTestModel : ConfigFileModel
    {

        [ConfigEntryDescription(Value = "This is the model's name")]
        [ConfigEntryDefaultValue(Value = "Model A")]
        public ConfigEntry<string> ModelName { get; set; }

        [ConfigEntryDescription(Value = "This is the model's first value")]
        [ConfigEntryDefaultValue(Value = 10)]
        public ConfigEntry<int> ModelValue1 { get; set; }

        //An entry with a custom config file key, 
        //custom keys are not required; the property/variable name will be used instead if not set.
        [ConfigEntryDescription(Value = "This is the model's second value, with a custom key.")]
        [ConfigEntryKey(Value = "CustomKey_ModelValue2")]
        [ConfigEntryDefaultValue(Value = 5f)]
        public ConfigEntry<float> ModelValue2 { get; set; }

        //test with no entry description
        [ConfigEntryDefaultValue(Value = 2)]
        public ConfigEntry<int> ModelValue3 { get; set; }

        //test with no default value
        [ConfigEntryDescription(Value = "Hello")]
        public ConfigEntry<int> ModelValue4 { get; set; }

        //Enum tests
        [ConfigEntryDescription(Value = "This is an Enum")]
        [ConfigEntryDefaultValue(Value = TestEnum.IndexA)]
        public ConfigEntry<TestEnum> ModelValue5 { get; set; }

        //Unsupported Type test + wrong default value type
        [ConfigEntryDescription(Value = "This is an unsupported Object")]
        [ConfigEntryDefaultValue(Value = 20)]
        public ConfigEntry<object> ModelValue6 { get; set; }

        //Wrong default value type test: primitives
        [ConfigEntryDescription(Value = "This is primitive type with the wrong default value type")]
        [ConfigEntryDefaultValue(Value = 1.5f)]
        public ConfigEntry<int> ModelValue7 { get; set; }

        //Wrong default value type test: enum
        [ConfigEntryDescription(Value = "This is an Enum with a bad default value")]
        [ConfigEntryDefaultValue(Value = 2000)]
        public ConfigEntry<TestEnum> ModelValue8 { get; set; }

        //Constructor call, you normally leave this empty. Optiona, you can use Bind<T> instead to pass the information.
        public ConfigModelTestModel(ConfigFile file = null, string section = null, ManualLogSource logger = null) : base(file, section, logger) { }

        //-----Virtual helper method examples-----//

        /// <summary>
        /// An example of OnModelCreate. This is called in the constructor and can be used to modify the ConfigFile settings and section name before properties are bound. Typically used to change the section name from what is set in the attributes.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sectionName"></param>
        protected override void OnModelCreate(ConfigFile file, ref string sectionName)
        {
            sectionName = "Example_Section";    //Example override of the attribute.
            //base.OnModelCreate(file, ref sectionName); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// This is called on each ConfigEntry<> property BEFORE it is linked/bound to the Config File. You can choose to optionally skip the standard binding process altogether by setting [useStandardBindingMethod=false]. You can use this to change basically everything but the Type of the default value. You will need to make use of Reflection or IF statements to handle [defaultValue].
        /// </summary>
        protected override void PrePropertyBind<T>(PropertyInfo property, ref string sectionName, ref string key, ref T defaultValue, ConfigDescription description, ref ConfigFile file, ref bool useStandardBindingMethod)
        {           
            //base.PrePropertyBind(property, ref sectionName, ref key, ref defaultValue, description, ref file, ref useStandardBindingMethod); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// This is called on each ConfigEntry<> property AFTER it has been linked/bound via ConfigFile.Bind(). Note that changes made here will NOT affect the value if the ConfigEntry<> property itself. This is intended to be used for other setup purposes that rely on the property's config file value. You will need to make use of Reflection or IF statements to handle [value].
        /// </summary>
        protected override void PostPropertyBind<T>(ConfigEntry<T> value, string sectionName, string key, ConfigFile file)
        {
            //base.PostPropertyBind(value, sectionName, key, file); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// An event hook for ConfigFile.configReloaded. This is called whenever BepInEx reloads all configuration files.
        /// </summary>
        public override void OnConfigReloaded(object sender, EventArgs e)
        {
            //base.OnConfigReloaded(sender, e); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// An event hook for ConfigFile.settingChanged. This is called whenever BepInEx's internal configuration settings have been changed.
        /// </summary>
        public override void OnSettingsChanged(object sender, SettingChangedEventArgs args)
        {
            //base.OnSettingsChanged(sender, args); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// This is called whenever the active ConfigFile is changed by calling ChangeConfigFile(). This is called AFTER the change has been completed. you can use this to handle any clean up or other data you would like to move to/from the different config files.
        /// </summary>
        /// <param name="oldFile">The old ConfigFile.</param>
        /// <param name="newFile">The current/new ConfigFile.</param>
        protected override void OnConfigFileMigration(ConfigFile oldFile, ConfigFile newFile)
        {
            //base.OnConfigFileMigration(oldFile, newFile); //This does nothing and can be ommitted.
        }

        /// <summary>
        /// This is called after a ConfigFile.ConfigReloaded event has been triggered. It will be called once for EACH model property that has been orphaned (their values couldn't not be read from the config file). You can use this to deal with ConfigEntries that may have bad values set to them.
        /// </summary>
        /// <typeparam name="T">The inner type of the ConfigEntry<> </typeparam>
        /// <param name="orphanedEntry">The orphaned ConfigEntry.</param>
        /// <param name="sectionName">The section name this ConfigEntry is under. Is equal to Model.SectionName.</param>
        /// <param name="oprhanDictConfigDef">The raw ConfigDefinition value from ConfigFile.OrphanedEntries.</param>
        /// <param name="orphanDictStringValue">The raw string value from ConfigFile.OrphanedEntries.</param>
        /// <param name="file">The current ConfigFile.</param>
        public override void OrphanedPropertyPostConfigReload<T>(ConfigEntry<T> orphanedEntry, string sectionName, ConfigDefinition oprhanDictConfigDef, string orphanDictStringValue, ConfigFile file)
        {
            //base.OrphanedPropertyPostConfigReload(orphanedEntry, sectionName, oprhanDictConfigDef, orphanDictStringValue, file); //This does nothing and can be ommitted.
        }
    }

    public enum TestEnum
    {
        IndexA = -1,
        IndexB = 0,
        IndexC = 1,
        IndexD = 2
    }
}
