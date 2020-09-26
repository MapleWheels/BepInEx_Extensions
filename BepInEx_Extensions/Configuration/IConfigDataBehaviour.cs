using BepInEx.Configuration;
using BepInEx.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigDataBehaviour<T>
    {
        event Action<ConfigFile, IConfigData<T>> PreBind;
        event Action<ConfigFile, IConfigData<T>> PostBind;
    }
}
