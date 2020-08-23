using BepInEx;
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;
using BepInEx.Logging;
using ConfigModelTests.Tests;
using System;
using System.Reflection;

namespace ConfigModelTests
{
    /// <summary>
    /// A quick test of the ConfigFileModel system. NOTE/TODO: Refactor tests to be more thoughrough. NOTE: Unit Testing package not included to reduce repository bloat. 
    /// </summary>
    [BepInPlugin("lib.test.configmodeltests", "ConfigModelTests", "0.0.1")]
    public class ModelTesterPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogWarning("ConfigModelTests: With Logger");
            Test(Config, Logger);
            Logger.LogWarning("ConfigModelTests: Without Logger");
            Test(Config);
        }

        public void Test(ConfigFile file, ManualLogSource logger = null)
        {
            try
            {
                ConfigModelTestModel cmt = new ConfigModelTestModel(file, "TestSection", logger).Bind<ConfigModelTestModel>();  //Option A
                cmt = new ConfigModelTestModel().Bind<ConfigModelTestModel>(file, "TestSection2", logger);  //Option B

                //Option C
                ConfigModelTestModel cmt2 = Config.BindModel<ConfigModelTestModel>(logger, "TestSection3");

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
