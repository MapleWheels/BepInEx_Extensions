using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;
using BepInEx;

namespace ConfigModelTests.Tests
{
    [BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
    public class CDMTestPlugin : BaseUnityPlugin
    {
        CDTestModel model;

        void Awake()
        {
            model = Config.BindModel<CDTestModel>(Logger);
            Logger.LogInfo($"CDM Tests: model init completed.");
            Logger.LogInfo($"CDM Tests: model.ConfigOption1={model.ConfigOption1.Value}");
            model.ConfigOption1.Value = 20f;
            Logger.LogInfo($"CDM Tests: model.ConfigOption1={model.ConfigOption1.Value}");
            //Unitialised entry test, late init.
            Logger.LogInfo($"CDM Tests Pre-Init: model.ConfigOption2={model.ConfigOption2.Value}");
            model.ConfigOption2 = new ConfigData<float>()
            {
                DefaultValue = 10f,
                DescriptionString = "hello",
                SectionName = model.SectionName
            }.Bind(Config, Logger);

            Logger.LogInfo($"CDM Tests Post-Init: model.ConfigOption2={model.ConfigOption2.Value}");
        }
    }
}
