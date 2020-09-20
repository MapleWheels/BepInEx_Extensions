using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public static class CFMExtensions
    {
        public static T BindModel<T>(this ConfigFile file, ManualLogSource logger = null) where T : ConfigFileModel
        {
            T cfm = Activator.CreateInstance<T>();
            cfm.BindToConfig(file, logger);
            return cfm;
        }

        public static void BindCFMEntry<T>(this CFMEntry<T> entry) where T : System.IEquatable<T>, System.IComparable
        {

        }
    }
}
