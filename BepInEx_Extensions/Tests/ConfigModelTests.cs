using BepInEx.Configuration;
using System;
using System.Reflection;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;

/// <author> PerfidiousLeaf </author>
/// <date>2020-08-13</date>
namespace BepInEx.Extensions.Tests
{
    /// <summary>
    /// A ConfigFileModel example use case and model test.
    /// </summary>
    [ConfigModelSectionName(Value = "This_Is_An_Example_Test_Section")] //This gets changed in OnModelCreate()
    public class ConfigModelTests : ConfigFileModel
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

        //Constructor call, you normally leave this empty.
        public ConfigModelTests(ConfigFile file, ManualLogSource logger = null, string section = null) : base(file, logger, section) { }

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
        protected override void PostPropertyBind<T>(T value, string sectionName, string key, ConfigFile file)
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
    }

    public enum TestEnum
    {
        IndexA = -1,
        IndexB = 0,
        IndexC = 1,
        IndexD = 2
    }


    /// <summary>
    /// A quick test of the ConfigFileModel system. NOTE/TODO: Refactor tests to be more thoughrough. NOTE: Unit Testing package not included to reduce repository bloat. 
    /// </summary>
    public class ModelTesterPlugin
    {
        public void Test(ConfigFile file, ManualLogSource logger = null)
        {
            try
            {
                ConfigModelTests cmt = new ConfigModelTests(file, logger, "TestSection");
                logger.LogInfo($"ModelTesterPlugin::Awake() | TypeOf({nameof(cmt)})={cmt.GetType()}");
                logger.LogInfo($"ConfigModelTest: ModelName = {cmt.ModelName.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue1 = {cmt.ModelValue1.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue2 = {cmt.ModelValue2.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue3 = {cmt.ModelValue3.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue4 = {cmt.ModelValue4.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue5 = {cmt.ModelValue5.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue6 = {cmt.ModelValue6.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue7 = {cmt.ModelValue7.Value}");
                logger.LogInfo($"ConfigModelTest: ModelValue8 = {cmt.ModelValue8.Value}");
            }
            catch (TargetException te)
            {
                logger.LogError("ModelTesterPlugin::Test() | Initalization fail.");
                logger.LogError(te.Source);
                logger.LogError(te.StackTrace);
                logger.LogError(te.Message);
                logger.LogError("TargetException: InnerException Data.");
                logger.LogError(te.InnerException);
                logger.LogError(te.InnerException?.Source);
                logger.LogError(te.InnerException?.StackTrace);
                logger.LogError(te.InnerException?.Message);
            }
            catch (TypeLoadException tle)
            {
                logger.LogError("ModelTesterPlugin::Test()");
                logger.LogError(tle.Source);
                logger.LogError(tle.StackTrace);
                logger.LogError(tle.Message);
                logger.LogError("TypeLoadException: InnerException Data.");
                logger.LogError(tle.InnerException);
                logger.LogError(tle.InnerException?.Source);
                logger.LogError(tle.InnerException?.StackTrace);
                logger.LogError(tle.InnerException?.Message);
            }
            catch (Exception e)
            {
                logger.LogError("ModelTesterPlugin::Test() | Generic exception.");
                logger.LogError(e.Source);
                logger.LogError(e.StackTrace);
                logger.LogError(e.Message);
                logger.LogError("Exception: InnerException Data.");
                logger.LogError(e.InnerException);
                logger.LogError(e.InnerException?.Source);
                logger.LogError(e.InnerException?.StackTrace);
                logger.LogError(e.InnerException?.Message);
            }
        }
    }
}
