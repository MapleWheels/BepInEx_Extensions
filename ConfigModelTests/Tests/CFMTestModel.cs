using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Extensions.Configuration;

namespace ConfigModelTests.Tests
{
    public class CFMTestModel : ConfigFileModel
    {
        public CFMEntry<float> ConfigOption1 = new CFMEntry<float>()
        {
            DefaultKey = nameof(ConfigOption1),
            DefaultValue = 10f,
            DescriptionString = "Test"
        };
    }
}
