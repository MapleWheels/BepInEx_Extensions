﻿using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

namespace ConfigModelTests.Tests
{
    /// <summary>
    /// Test model for ConfigDataModel functionality.
    /// </summary>
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
                
            ConfigOption1.PreBind += (ConfigFile file) =>
            {
                Logger?.LogWarning($"Pre bind called for ConfigOption1");
            };

            ConfigOption1.PostBind += (ConfigFile file) =>
            {
                Logger?.LogWarning($"Test plugin loaded: { ConfigOption1.Key } value is { ConfigOption1.Value } post-bind");
            };
        }

        public override void OnModelCreate(ConfigFile config)
        {
            SectionName = "wololoo";
            Logger?.LogWarning("Debug: CDTestModel::OnModelCreate() was run.");
        }
    }
}
