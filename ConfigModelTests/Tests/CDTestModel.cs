using BepInEx.Extensions.Configuration;
using BepInEx.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigModelTests.Tests
{
    public class CDTestModel : ConfigDataModel
    {
        public ConfigData<float> ConfigOption1 { get; set; }
        public ConfigData<float> ConfigOption2 { get; set; }    //Uninitialised.

        public override void SetDefaults()
        {
            SectionName = "Default";

            Logger.LogWarning($"Test ConfigData: SetDefaults started");

            ConfigOption1 = new ConfigData<float>()
            {
                DefaultValue = 10f,
                DescriptionString = "This is a config variable",
                AcceptableValues = new AcceptableValueRange<float>(0.0f, 100.0f),
            };

            Logger.LogWarning($"Test ConfigData Co1: {ConfigOption1}");
                
            ConfigOption1.PreBind += (ConfigFile file, IConfigData<float> cfg) =>
            {
                Logger?.LogWarning($"Pre bind called for ConfigOption1");
            };

            ConfigOption1.PostBind += (ConfigFile file, IConfigData<float> cfg) =>
            {
                Logger?.LogWarning($"Test plugin loaded: { cfg.Key } value is { cfg.Value } post-bind");
            };
        }
    }
}
