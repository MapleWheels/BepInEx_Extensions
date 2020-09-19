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
        public string DefaultKey { get; } = null;
        public T DefaultValue { get; } = default(T);
        public string DescriptionString { get; }
        public AcceptableValueList<T> AcceptableList { get; }
        public AcceptableValueRange<T> AcceptableRange { get; }
        public object[] Tags { get; }
    }
}
