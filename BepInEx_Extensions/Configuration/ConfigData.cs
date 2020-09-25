using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx.Configuration;
using BepInEx.Logging;

namespace BepInEx.Extensions.Configuration
{
    public class ConfigData<T> : IConfigData<T>
    {
        public ManualLogSource LogSource { get; set; }
        /// <summary>
        /// The BepInEx ConfigEntry that this property is currently bound to.
        /// </summary>
        public ConfigEntry<T> Entry { get; protected set; }
        public T Value
        {
            get
            {
                if (Entry == null)
                {
                    LogSource?.LogError($"{Key}: ConfigData.Entry.Value is null. Returning Type Defaults.");
                    return default;
                }
                return Entry.Value;
            }
            set
            {
                if (Entry == null)
                {
                    LogSource?.LogError($"{Key}: ConfigData.Entry.Value is null. No value assigned.");
                    return;
                }
                Entry.Value = value;
            }
        }
        /// <summary>
        /// The currently bound config file.
        /// </summary>
        public ConfigFile Config { get => Entry.ConfigFile; }
        public string SectionName { get; set; }

        public string DescriptionString { get; set; }

        public string Key { get; set; }

        public T DefaultValue { get; set; }

        public AcceptableValueBase AcceptableValues { get; set; }

        public object[] Tags { get; set; }

        private event EventHandler _OnSettingChangedInternal; 
        /// <summary>
        /// Wrapper for ConfigFile.SettingChanged. Called on BepInEx config reload.
        /// </summary>
        public virtual event EventHandler OnSettingChanged
        {
            add
            {
                _OnSettingChangedInternal -= value;
                _OnSettingChangedInternal += value;
            }
            remove
            {
                _OnSettingChangedInternal -= value;
            }
        }

        private event Action<ConfigFile, IConfigData<T>> _PreBindInternal;
        /// <summary>
        /// Called right before this config is bound. Can be used to make changes to the config bind info.
        /// </summary>
        public virtual event Action<ConfigFile, IConfigData<T>> PreBind
        {
            add
            {
                _PreBindInternal -= value;
                _PreBindInternal += value;
            }
            remove
            {
                _PreBindInternal -= value;
            }
        }
        
        private event Action<ConfigFile, IConfigData<T>> _PostBindInternal;
        /// <summary>
        /// Called immediately after the config is bound to the ConfigFile.
        /// </summary>
        public virtual event Action<ConfigFile, IConfigData<T>> PostBind
        {
            add
            {
                _PostBindInternal -= value;
                _PostBindInternal += value;
            }
            remove
            {
                _PostBindInternal -= value;
            }            
        }

        /// <summary>
        /// Binds the ConfigData to the supplied ConfigFile. altX variables will only be used if the instance value for the current object is null.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="altSectionName"></param>
        /// <param name="altKey"></param>
        /// <param name="altDesc"></param>
        /// <param name="altDefaultValue"></param>
        public IConfigBindable<T> Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue)
        {
            //Exception checks
            if (logger == null)
            {
                if (LogSource == null)
                {
                    logger = Logger.CreateLogSource("default");
                    LogSource = logger;
                    logger.LogWarning($"ConfigData::Bind() | Logger arg is null! Using 'default'.");
                }
                else
                {
                    logger = LogSource;
                }
            }

            if (config == null)
            {
                logger.LogError($"ConfigData::Bind() | ConfigFile arg is null! Aborting!.");
                return this;
            }

            //--Run Pre-Bind code
            //So Invoke reflection makes a shadow copy at some point so even ref types need to be copied back.
            ConfigData<T> arg = this;

            this._PreBindInternal?.Invoke(config, arg);

            //Reassignment
            this.Entry = arg.Entry;
            this.AcceptableValues = arg.AcceptableValues;
            this.DefaultValue = arg.DefaultValue;
            this.DescriptionString = arg.DescriptionString;
            this.Key = arg.Key;
            this.SectionName = arg.SectionName;
            this.Tags = arg.Tags;

            //Sanity checks
            //Section name
            if (this.SectionName == null)
                this.SectionName = altSectionName;
            if (this.SectionName == null)
            {
                logger.LogWarning($"ConfigData::Bind() | SectionName arg is null! Using 'default'.");
                this.SectionName = "default";
            }
            //Bind key name
            if (this.Key == null)
                this.Key = altKey;
            if (this.Key == null)
            {
                logger.LogWarning($"ConfigData::Bind() | Key arg is null! Using 'default'.");
                this.Key = "default";
            }

            //Description
            if (this.DescriptionString == null)
                this.DescriptionString = altDesc;
            if (this.DescriptionString == null)
            {
                logger.LogWarning($"ConfigData::Bind() | DescriptionString arg is null! Using '<None>'.");
                this.DescriptionString = "<None>";
            }

            if (DefaultValue == null || DefaultValue.Equals(default))
                DefaultValue = altDefaultValue;
            if (DefaultValue == null || DefaultValue.Equals(default))
            {
                logger.LogWarning($"ConfigData::Bind() | DefaultValue is not set. Using 'default'");
                DefaultValue = default; //might be null
            }

            if (this.Entry == null)
                this.Entry = config.Bind(SectionName, Key, DefaultValue, new ConfigDescription(DescriptionString, AcceptableValues, Tags));

            //Once more, Post-Bind Code
            arg = this;

            this._PostBindInternal?.Invoke(config, arg);

            //Reassignment
            this.Entry = arg.Entry;
            this.AcceptableValues = arg.AcceptableValues;
            this.DefaultValue = arg.DefaultValue;
            this.DescriptionString = arg.DescriptionString;
            this.Key = arg.Key;
            this.SectionName = arg.SectionName;
            this.Tags = arg.Tags;
            this.Value = arg.Value;

            PostBindInternal();
            return this;
        }

        private void PostBindInternal()
        {
            Entry.SettingChanged += (object o, EventArgs e) => this._OnSettingChangedInternal?.Invoke(o, e);
        }
    }

    public static class CDExtensions
    {
        public static ConfigData<T> Bind<T>(this ConfigData<T> configData, ConfigFile config, ManualLogSource logger) =>
            (ConfigData<T>)((IConfigBindable<T>)configData).Bind(config, logger, null, null, null, default);
    }
}
