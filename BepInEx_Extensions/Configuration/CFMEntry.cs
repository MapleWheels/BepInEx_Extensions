using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public class CFMEntry<T> : IConfigEntryData<T>, IConfigEntryAcceptableValueListData<T>, IConfigEntryAcceptableValueRangeData<T>, IConfigEntryTagsData where T : System.IEquatable<T>, System.IComparable
    {
        public ConfigEntry<T> RawEntry { get; set; }
        public virtual T Value { get => RawEntry.Value; set => RawEntry.Value = value; }
        public string DefaultKey { get; set; }
        public T DefaultValue { get; set; }
        public string DescriptionString { get; set; }
        public AcceptableValueList<T> AcceptableList { get; set; }
        public AcceptableValueRange<T> AcceptableRange { get; set; }
        public object[] Tags { get; set; }
    }
}
