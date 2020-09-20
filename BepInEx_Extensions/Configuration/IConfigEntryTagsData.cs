using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigEntryTagsData
    {
        object[] Tags { get; set; }
    }
}
