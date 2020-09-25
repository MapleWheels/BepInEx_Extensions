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
            model.ConfigOption1.Value = 100f;
            Logger.LogInfo($"CDM Tests: model.ConfigOption1={model.ConfigOption1.Value}");
        }
    }
}
