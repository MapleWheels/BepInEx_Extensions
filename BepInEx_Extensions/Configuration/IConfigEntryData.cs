using BepInEx.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigEntryData<T>
    {
        ConfigEntry<T> RawEntry { get; set; }
        T DefaultValue { get; }
        string DefaultKey { get; }
        string DescriptionString { get; }
    } 
}
