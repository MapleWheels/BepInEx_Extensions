using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public abstract class ConfigFileModel : IConfigModelData
    {
        private string _SectionName;
        public string SectionName 
        {
            get
            {
                if (_SectionName == null || _SectionName == "")
                {
                    if (Logger != null)
                        Logger.LogWarning($"CFM Error - {this.GetType().Name} | SectionName not set. Using 'default'.");
                    _SectionName = "default";
                }
                return _SectionName;
            }
            set => _SectionName = value;
        }
        public ConfigFile ActiveConfigFile { get; protected set; }
        public ManualLogSource Logger { get; set; }

        #region user_hooks
        public virtual void OnModelCreate() { }
        public virtual void PreBind<T>(CFMEntry<T> entry) where T : System.IEquatable<T>, System.IComparable { }
        public virtual void PostBind<T>(CFMEntry<T> entry) where T : System.IEquatable<T>, System.IComparable { }
        #endregion

        #region functions        
        public void BindToConfig(ConfigFile file, ManualLogSource logger = null)
        {
            
        }
        #endregion
    }
}
