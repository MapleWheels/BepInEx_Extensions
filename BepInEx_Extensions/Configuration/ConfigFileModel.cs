using BepInEx.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BepInEx.Extensions.Configuration
{
    public abstract class ConfigFileModel : IConfigModelData
    {
        public string SectionName { get; }
        private ConfigFile _ActiveConfigFile { get; set; }
        public ConfigFile ActiveConfigFile { get => _ActiveConfigFile; }
    }
}
