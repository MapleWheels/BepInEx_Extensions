
using BepInEx.Extensions.Configuration;

namespace ConfigModelTests.Tests
{
    public class CDTestStaticModel : ConfigDataModel
    {
        public static ConfigData<float> var1 = new ConfigData<float>()
        {
            DefaultValue = 1f,
            DescriptionString = "Testing static fields"
        };

        public static ConfigData<string> var2 = new ConfigData<string>()
        {
            DefaultValue = "henlo",
            DescriptionString = "Testing static fields"
        };
    }
}
