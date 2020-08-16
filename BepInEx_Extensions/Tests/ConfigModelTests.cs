using BepInEx;
using BepInEx.Configuration;
using System;
using System.Reflection;
using BepInEx_Extensions.Configuration;

namespace BepInEx_Extensions.Tests
{
    /// <summary>
    /// A ConfigFileModel example use case and model test.
    /// </summary>
    /// <author> PerfidiousLeaf </author>
    /// <date>2020-08-13</date>
    /// <version>1.0.0</version>
    public class ConfigModelTests : ConfigFileModel
    {

        [ConfigEntryDesc(Value = "This is the model's name")]
        [ConfigDefaultValue(Value = "Model A")]
        public ConfigEntry<string> ModelName { get; set; }

        [ConfigEntryDesc(Value = "This is the model's first value")]
        [ConfigDefaultValue(Value = 10)]
        public ConfigEntry<int> ModelValue1 { get; set; }

        [ConfigEntryDesc(Value = "This is the model's second value")]
        [ConfigDefaultValue(Value = 5f)]
        public ConfigEntry<float> ModelValue2 { get; set; }

        //test with no entry description
        [ConfigDefaultValue(Value = 2)]
        public ConfigEntry<int> ModelValue3 { get; set; }
        //test with no default value
        [ConfigEntryDesc(Value = "Hello")]
        public ConfigEntry<int> ModelValue4 { get; set; }
        //Enum tests
        [ConfigEntryDesc(Value = "This is an Enum")]
        [ConfigDefaultValue(Value = ItemIndex.Syringe)]
        public ConfigEntry<ItemIndex> ModelValue5 { get; set; }
        //Unsupported Type test + wrong default value type
        [ConfigEntryDesc(Value = "This is an unsupported Object")]
        [ConfigDefaultValue(Value = 20)]
        public ConfigEntry<CharacterBody> ModelValue6 { get; set; }
        //Wrong default value type test: primitives
        [ConfigEntryDesc(Value = "This is primitive type with the wrong default value")]
        [ConfigDefaultValue(Value = 1.5f)]
        public ConfigEntry<int> ModelValue7 { get; set; }
        //Wrong default value type test: enum
        [ConfigEntryDesc(Value = "This is an Enum")]
        [ConfigDefaultValue(Value = 2000)]
        public ConfigEntry<ItemIndex> ModelValue8 { get; set; }

        public ConfigModelTest(ConfigFile file, string section) : base(file, section) { }

    }

    /// <summary>
    /// A quick test of the ConfigFileModel system.
    /// </summary>
    /// <author> PerfidiousLeaf </author>
    /// <date>2020-08-13</date>
    /// <version>1.0.0</version>
    [BepInPlugin("ca.tbn.expermental", "BepinEx_ConfigModelFiles", "0.0.1")]
    public class ModelTesterPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            try
            {
                ConfigModelTest cmt = new ConfigModelTest(Config, "TestSection");
                UnityEngine.Debug.Log("ConfigModelTest: ModelName = " + cmt.ModelName.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue1 = " + cmt.ModelValue1.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue2 = " + cmt.ModelValue2.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue3 = " + cmt.ModelValue3.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue4 = " + cmt.ModelValue4.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue5 = " + cmt.ModelValue5.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue6 = " + cmt.ModelValue6.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue7 = " + cmt.ModelValue7.Value);
                UnityEngine.Debug.Log("ConfigModelTest: ModelValue8 = " + cmt.ModelValue8.Value);
                UnityEngine.Debug.Log("ModelTesterPlugin::Awake() | TypeOf(cmt)=" + cmt.GetType());
            }
            catch (TargetException te)
            {
                UnityEngine.Debug.LogError("ModelTesterPlugin::Awake() | Initalization fail.");
                UnityEngine.Debug.Log(te.Source);
                UnityEngine.Debug.Log(te.StackTrace);
                UnityEngine.Debug.Log(te.Message);
                UnityEngine.Debug.Log("TargetException: InnerException Data.");
                UnityEngine.Debug.Log(te.InnerException);
                UnityEngine.Debug.Log(te.InnerException?.Source);
                UnityEngine.Debug.Log(te.InnerException?.StackTrace);
                UnityEngine.Debug.Log(te.InnerException?.Message);
            }
            catch (TypeLoadException tle)
            {
                UnityEngine.Debug.LogError("ModelTesterPlugin::Awake()");
                UnityEngine.Debug.LogError(tle.Source);
                UnityEngine.Debug.LogError(tle.StackTrace);
                UnityEngine.Debug.LogError(tle.Message);
                UnityEngine.Debug.Log("TypeLoadException: InnerException Data.");
                UnityEngine.Debug.Log(tle.InnerException);
                UnityEngine.Debug.Log(tle.InnerException?.Source);
                UnityEngine.Debug.Log(tle.InnerException?.StackTrace);
                UnityEngine.Debug.Log(tle.InnerException?.Message);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("ModelTesterPlugin::Awake() | Generic exception.");
                UnityEngine.Debug.Log(e.Source);
                UnityEngine.Debug.Log(e.StackTrace);
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log("Exception: InnerException Data.");
                UnityEngine.Debug.Log(e.InnerException);
                UnityEngine.Debug.Log(e.InnerException?.Source);
                UnityEngine.Debug.Log(e.InnerException?.StackTrace);
                UnityEngine.Debug.Log(e.InnerException?.Message);
            }
        }
    }
}
