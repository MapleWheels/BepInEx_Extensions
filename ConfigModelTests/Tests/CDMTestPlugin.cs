using BepInEx;
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

namespace ConfigModelTests.Tests
{
    [BepInPlugin("dev.cdmtests", "CDM Tests", "0.0.0")]
    public class CDMTestPlugin : BaseUnityPlugin
    {
        CDTestModel model;
        CDTestStaticModel model2;

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

            IConfigArray<int> testArray = Config.BindArray<int>(Logger, 
                size: 20, section: "arraytest", key: "testArray", description: "description", defaultValue: 10);
            Logger.LogInfo($"Array test, index: 3 | value={ testArray[3].Value }");

            ConfigEntry<bool> test = Config.Bind<bool>("standardInitTest", "test1", false);

            model2 = Config.BindModel<CDTestStaticModel>(Logger);

            Logger.LogInfo($"CDM Tests Post-Init: STATIC model2.var1={ (float) CDTestStaticModel.var1 }");
            Logger.LogInfo($"CDM Tests Post-Init: model2.var2={ (string) model2.var2 }");
        }
    }
}
