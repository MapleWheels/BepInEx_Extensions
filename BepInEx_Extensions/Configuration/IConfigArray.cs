using BepInEx.Configuration;
using BepInEx.Logging;

using System;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigArray<T> : IConfigBindable<T>
    {
        IConfigData<T> this[int index]
        {
            get;
        }

        event EventHandler OnSettingChanged;

        event System.Action<ConfigFile, IConfigData<T>> PreBind, PostBind;

        new IConfigArray<T> Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue);
    }
}
