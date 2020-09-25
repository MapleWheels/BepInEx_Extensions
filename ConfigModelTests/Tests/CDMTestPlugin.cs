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
            //Unitialised entry test.
            Logger.LogInfo($"CDM Tests: model.ConfigOption2={model.ConfigOption2.Value}");
        }
    }
}
