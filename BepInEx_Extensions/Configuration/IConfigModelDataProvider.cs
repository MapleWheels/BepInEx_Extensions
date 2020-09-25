using BepInEx.Configuration;
using BepInEx.Logging;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigModelDataProvider
    {
        string SectionName { get; }
        ConfigFile Config { get; }
        ManualLogSource Logger { get; }
    }
}
