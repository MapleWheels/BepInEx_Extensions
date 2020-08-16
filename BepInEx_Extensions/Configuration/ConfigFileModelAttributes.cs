using System;

namespace BepInEx_Extensions.Configuration
{
    /// <summary>
    /// Intended for use with ConfigEntry<> vars in ConfigFileModel. Used to define the in-file descriptions when binding to the config file.
    /// </summary>
    /// <author> PerfidiousLeaf </author>
    /// <date>2020-08-13</date>
    /// <version>1.0.0</version>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigEntryDesc : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// Intended for use with ConfigEntry<> vars in ConfigFileModel. Used to define the default value when binding to the config file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigDefaultValue : Attribute
    {
        public object Value { get; set; }
    }

    
}
