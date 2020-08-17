using System;

/// <author>PerfidousLeaf | MapleWheels</author>
/// <date>2020-08-13</date>

namespace BepInEx.Extensions.Configuration
{
    /// <summary>
    /// Intended for use with ConfigEntry<> vars in ConfigFileModel. Used to define the in-file descriptions when binding to the config file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigEntryDescription : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// Intended for use with ConfigEntry<> vars in ConfigFileModel. Used to define the default value when binding to the config file. The default value for the type will be used if this attribute is ommitted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigEntryDefaultValue : Attribute
    {
        public object Value { get; set; }
    }

    /// <summary>
    /// Intended for use with ConfigEntry<> vars in ConfigFileModel. Used to define the Key/Name that will be used in the config file. The property name will be used instead if this attribute is ommitted.
    /// </summary>
    public class ConfigEntryKey : Attribute
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// Intended for use with ConfigFileModel. Used to define the section that the ConfigEntry<> properties in the Model-Class will be placed under. Used by ConfigFile.Bind().
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigModelSectionName : Attribute
    {
        public string Value { get; set; }
    }
}
