using BepInEx.Configuration;
using BepInEx.Logging;

using System;

namespace BepInEx.Extensions.Configuration
{
    public sealed class ConfigArray<T> : IConfigArray<T>
    {
        private ManualLogSource Logger;
        private IConfigData<T>[] _BackingArray;
        public IConfigData<T> this[int index] => _BackingArray[index];

        public ConfigArray(int size) => InitArray(size);

        public string SectionName { get; set; }
        public string DescriptionString { get; set; }
        public string Key { get; set; }
        public T DefaultValue { get; set; }
        public AcceptableValueBase AcceptableValues { get; set; }
        public object[] Tags { get; set; }

        public event EventHandler OnSettingChanged;
        public event Action<ConfigFile, IConfigData<T>> PreBind;
        public event Action<ConfigFile, IConfigData<T>> PostBind;

        public IConfigArray<T> Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue, object[] tags)
        {
            this.Tags = tags;
            return this.Bind(config, logger, altSectionName, altKey, altDesc, altDefaultValue);
        }

        public IConfigArray<T> Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue)
        {
            if (logger == null)
            {
                if (this.Logger == null)
                    this.Logger = Logging.Logger.CreateLogSource("default");
            }
            else
                this.Logger = logger;
            

            if (config == null)
            {
                Logger.LogError("ConfigArray::Bind() ConfigFile is null. Aborting!");
                return this;
            }

            if (altSectionName != null)
                this.SectionName = altSectionName;
            if (altKey != null)
                this.Key = altKey;
            if (altDefaultValue != null && !altDefaultValue.Equals(default(T)))
                DefaultValue = altDefaultValue;

            for (int ind = 0; ind < _BackingArray.Length; ind++)
            {
                if (_BackingArray[ind] == null)
                    _BackingArray[ind] = new ConfigData<T>();

                _BackingArray[ind].DescriptionString = this.DescriptionString;
                _BackingArray[ind].DefaultValue = this.DefaultValue;
                _BackingArray[ind].AcceptableValues = this.AcceptableValues;
                _BackingArray[ind].Tags = this.Tags;
                _BackingArray[ind].Key = $"{this.Key}_{ind}";
                _BackingArray[ind].SectionName = this.SectionName;

                _BackingArray[ind].Bind(config, logger, null, null, null, this.DefaultValue);
            }

            return this;
        }

        IConfigBindable<T> IConfigBindable<T>.Bind(ConfigFile config, ManualLogSource logger, string altSectionName, string altKey, string altDesc, T altDefaultValue) => this.Bind(config, logger, altSectionName, altKey, altDesc, altDefaultValue);
        

        private void InitArray(int size) 
        {
            _BackingArray = new ConfigData<T>[size];
            for(int i=0; i<size; i++)
                _BackingArray[i] = new ConfigData<T>();
        }
    }

    public static class CAExtensions
    {
        public static ConfigArray<T> BindArray<T>(this ConfigFile file, ManualLogSource logger, int size, string section, string key, string description = "", T defaultValue = default, object[] tags = null) 
        => (ConfigArray<T>)new ConfigArray<T>(size).Bind(file, logger, section, key, description, defaultValue, tags);
        
    }
}
