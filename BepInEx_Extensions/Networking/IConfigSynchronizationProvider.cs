using BepInEx.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.Extensions.Networking
{
    public interface IConfigSynchronizationProvider
    {
        void Initialize();
        void SynchronizeDataEntry<T>(IConfigData<T> entry);
    }
}
