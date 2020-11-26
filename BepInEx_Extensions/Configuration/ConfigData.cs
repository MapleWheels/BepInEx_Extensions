using BepInEx.Configuration;
using BepInEx.Logging;

using System;

namespace BepInEx.Extensions.Configuration
{
    /// <summary>
    /// Allows you to define config file entries and all of the binding properties. Intended to be declared inside of a class that implements IConfigModelBehaviour.
    /// </summary>
    /// <typeparam name="T">the type of the config variable.</typeparam>
    public sealed class ConfigData<T> : IConfigData<T>
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
        /// <summary>
        /// The section name this will be bound to when Bind() is called.
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// The description that will be used for the Config Option in the config file entry.
        /// </summary>
        public string DescriptionString { get; set; }

        /// <summary>
        /// The key name string that this will be bound to in the config file.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The default value for the config file if none exists already.
        /// </summary>
        public T DefaultValue { get; set; }

        /// <summary>
        /// Should be assigned an AcceptableValuesRange<> or AcceptableValuesList<>.
        /// </summary>
        public AcceptableValueBase AcceptableValues { get; set; }

        /// <summary>
        /// Configuration tags used for add-on plugins such as the Configuration Manager.
        /// </summary>
        public object[] Tags { get; set; }

        private event EventHandler _OnSettingChangedInternal; 
        /// <summary>
        /// Wrapper for ConfigFile.SettingChanged. Called on BepInEx config reload.
        /// </summary>
        public event EventHandler OnSettingChanged
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

        private event Action<ConfigFile> _PreBindInternal;
        /// <summary>
        /// Called right before this config is bound. Can be used to make changes to the config bind info. IConfigData can be casted to ConfigData.
        /// </summary>
        public event Action<ConfigFile> PreBind
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
        
        private event Action<ConfigFile> _PostBindInternal;
        /// <summary>
        /// Called immediately after the config is bound to the ConfigFile. IConfigData can be casted to ConfigData.
        /// </summary>
        public event Action<ConfigFile> PostBind
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
            this._PreBindInternal?.Invoke(config);

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

            //Post-Bind event code

            this._PostBindInternal?.Invoke(config);

            PostBindInternal();
            return this;
        }

        private void PostBindInternal()
        {
            Entry.SettingChanged += (object o, EventArgs e) => this._OnSettingChangedInternal?.Invoke(o, e);
        }


        //Extensions
        public static implicit operator T(ConfigData<T> data) => data.Value;
    }

    public static class CDExtensions
    {
        /// <summary>
        /// Binds the ConfigData to the ConfigFile.
        /// </summary>
        /// <typeparam name="T">ConfigEntry primitive type</typeparam>
        /// <param name="configData"></param>
        /// <param name="config">The Configuration File to bind to.</param>
        /// <param name="logger">Target for log messages.</param>
        /// <returns></returns>
        public static ConfigData<T> Bind<T>(this ConfigData<T> configData, ConfigFile config, ManualLogSource logger) =>
            (ConfigData<T>)((IConfigBindable<T>)configData).Bind(config, logger, null, null, null, default);

        /// <summary>
        /// Helper method for subscribing to the ConfigData pre-bind event.
        /// </summary>
        /// <typeparam name="T">the ConfigData instance type</typeparam>
        /// <param name="data">the ConfigData instance.</param>
        /// <param name="action">Event handle to be invoked.</param>
        /// <returns></returns>
        public static ConfigData<T> PreBindSubscribe<T>(this ConfigData<T> data, Action<ConfigFile> action)
        {
            data.PreBind += action;
            return data;
        }

        /// <summary>
        /// Helper method for subscribing to the ConfigData post-bind event.
        /// </summary>
        /// <typeparam name="T">the ConfigData instance type</typeparam>
        /// <param name="data">the ConfigData instance.</param>
        /// <param name="action">Event handle to be invoked.</param>
        /// <returns></returns>
        public static ConfigData<T> PostBindSubscribe<T>(this ConfigData<T> data, Action<ConfigFile> action)
        {
            data.PostBind += action;
            return data;
        }
    }
}
