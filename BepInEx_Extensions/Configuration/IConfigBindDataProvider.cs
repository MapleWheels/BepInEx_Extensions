
using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigBindDataProvider<T>
    {
        string SectionName { get; set; }
        string DescriptionString { get; set; }
        string Key { get; set; }
        T DefaultValue { get; set; }
        AcceptableValueBase AcceptableValues { get; set; }
        object[] Tags { get; set; }
    }
}
