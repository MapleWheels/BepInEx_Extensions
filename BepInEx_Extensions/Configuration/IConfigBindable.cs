using BepInEx.Configuration;
using BepInEx.Logging;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigBindable<T> : IConfigBindDataProvider<T>, IConfigBindableTypeComparator
    {
        IConfigBindable<T> Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue);
    }

    public interface IConfigBindableTypeComparator { }
}
