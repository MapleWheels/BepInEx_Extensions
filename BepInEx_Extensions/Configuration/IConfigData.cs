using BepInEx.Configuration;

using System;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigData<T> : IConfigBindable<T>
    {
        T Value { get; set; }

        event EventHandler OnSettingChanged;
    }
}
