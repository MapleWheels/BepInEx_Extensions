using BepInEx.Configuration;
using BepInEx.Logging;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigModelBehaviour
    {
        void SetDefaults();
        void OnModelCreate(ConfigFile Config);
        void SetConfigFile(ConfigFile config);
        void BindModel(ConfigFile config, string sectionName, ManualLogSource logger);
    }
}
