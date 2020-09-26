using BepInEx;
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigModelTests.Example
{
    /// <summary>
    /// This is an example usage of the ConfigDataModel type and it's usages.
    /// </summary>
    [BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
    public class ExamplePlugin : BaseUnityPlugin
    {
        ExampleModel model;

        void Awake()
        {
            model = Config.BindModel<ExampleModel>(Logger); //Initialized and ready to use.

            Logger.LogInfo($"ExamplePlugin: model init completed.");
            Logger.LogInfo($"ExamplePlugin: model.ConfigOption1={model.ConfigOption1.Value}");

            model.ConfigOption2.Value = 20f;
            Logger.LogInfo($"ExamplePlugin: model.ConfigOption2={model.ConfigOption2.Value}");


            //If you didn't initialize an entry in your config model type, or you want to do it externally, you can do so here. 
            //If you forgot to do so, you will get back a default version of your ConfigData entry, unbound from any ConfigFile.
            model.ConfigOption3 = new ConfigData<int>()
            {
                DefaultValue = 10,
                DescriptionString = "hello",
                SectionName = model.SectionName,
            }.Bind(Config, Logger); //Bind call

            //Or for events
            model.ConfigOption3 = new ConfigData<int>()
            {
                Key = nameof(model.ConfigOption3),      //Have to set this manually as this will not be bound via the ConfigDataModel, instead it will be bound on it's own.
                DefaultValue = 10,
                DescriptionString = "hello",
                SectionName = model.SectionName,
            };
            model.ConfigOption3.PostBind += (ConfigFile config) =>
            {
                Logger?.LogInfo("ExamplePlugin: ConfigOption3 is now bound!");
            };
            model.ConfigOption3.Bind(Config, Logger);   //Bind call

            model.ConfigOption3.OnSettingChanged += (object o, EventArgs e) =>
            {
                Logger?.LogInfo("ExamplePlugin: ConfigOption3 got reloaded!");
            };

            //Want to access the ConfigEntry<T> used by BepInEx? Use Entry.
            model.ConfigOption3.Entry.Value = 1;
            Logger.LogInfo($"ExamplePlugin: model.ConfigOption3={model.ConfigOption3.Value}");

            //Want to change config files for profile support?
            ConfigFile profile2 = new ConfigFile(System.IO.Path.Combine(Paths.BepInExConfigPath, "ExamplePlugin", "profile2"), true);
            model.SetConfigFile(profile2);
        }
    }
}
