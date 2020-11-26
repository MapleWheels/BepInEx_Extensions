using BepInEx.Configuration;

using System;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigData<T> : IConfigBindable<T>
    {
        T Value { get; set; }
        ConfigFile Config { get; }
        ConfigEntry<T> Entry { get; }

        event EventHandler OnSettingChanged;
    }
}
