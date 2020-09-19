using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigModelData
    {
        string SectionName { get; }
        ConfigFile ActiveConfigFile { get; }
    }
}
