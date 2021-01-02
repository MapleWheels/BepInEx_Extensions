using BepInEx;
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;

using System;

namespace ConfigModelTests.Example
{
    /// <summary>
    /// This is an example usage of the ConfigDataModel type and it's usages.
    /// </summary>
    [BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
    public class ExamplePlugin : BaseUnityPlugin
    {
        //NOTE: Before you read this, please take a look at the ExampleModel.cs file. The below will make a lot more sense if you do.
        ExampleModel model;

        void Awake()
        {
            //NOTE: Before you read this, please take a look at the ExampleModel.cs file. The below will make a lot more sense if you do.
            model = Config.BindModel<ExampleModel>(Logger); //Initialized and ready to use.

            Logger.LogInfo($"ExamplePlugin: model init completed.");
            Logger.LogInfo($"ExamplePlugin: model.ConfigOption1={ model.ConfigOption1.Value }");

            model.ConfigOption2.Value = 20f;                                                              
            Logger.LogInfo($"ExamplePlugin: model.ConfigOption2={ (float) model.ConfigOption2 }");    //Explicit & implicit conversion is supported.

            Logger.LogInfo($"ExamplePlugin: model.ConfigOption3={ (int) model.ConfigOption3 }");    //All defaults. Value = 0.

            //Want to change config files for profile support?
            ConfigFile profile2 = new ConfigFile(System.IO.Path.Combine(Paths.BepInExConfigPath, "ExamplePlugin", "profile2"), true);
            model.SetConfigFile(profile2);
        }
    }
}
