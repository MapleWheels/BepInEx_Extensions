using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <author>PerfidousLeaf | MapleWheels</author>
/// <date>2020-08-23</date>
namespace BepInEx.Extensions.Configuration
{
    public static class ConfigFileExtensions
    {
        public static T BindModel<T>(this ConfigFile file, ManualLogSource logger = null, string sectionName = null) where T : ConfigFileModel => Activator.CreateInstance<T>().Bind<T>(file, sectionName, logger);
    }
}
