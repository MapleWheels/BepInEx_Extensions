using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Extensions.Configuration;

namespace ConfigModelTests.Example
{
    [BepInPlugin("com.example.configModelExample", "Config Model Example", "0.0.0")]
    public class CFMExamplePlugin : BaseUnityPlugin
    {
        void Awake()
        {
            //First instantiate and bind your config file.
            CFMExampleModel configFileInstance = Config.BindModel<CFMExampleModel>();

            //Now use it, it's ready to go!
            Logger.LogInfo($"Hey, the CFM Example Model works!");
            Logger.LogInfo($"ConfigVariable1 = {configFileInstance.ConfigVariable1.Value}");
            Logger.LogInfo($"ConfigVariable2 = {configFileInstance.ConfigVariable2.Value}");
            Logger.LogInfo($"ConfigVariable3 = {configFileInstance.ConfigVariable3.Value}");

            //Want to change the active config file (for example, to support profiles)? Rebind it.
            ConfigFile newConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "custom_config.cfg"), true);
            configFileInstance.ChangeConfigFile(newConfigFile);
        }
    }
}
