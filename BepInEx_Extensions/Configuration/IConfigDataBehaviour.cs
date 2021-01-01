using BepInEx.Configuration;

using System;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigDataBehaviour<T>
    {
        event Action<ConfigFile, IConfigData<T>> PreBind;
        event Action<ConfigFile, IConfigData<T>> PostBind;
    }
}
