using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public static class CFMExtensions
    {
        public static void BindCFMEntry<TVal, T>(this T CFMEntry, ConfigFile config, string section) where T : IConfigEntryData<TVal> where TVal : System.IEquatable<TVal>, System.IComparable 
        {
            object[] tags = null;
            AcceptableValueList<TVal> accList = null;
            AcceptableValueRange<TVal> accRange = null;

            if (CFMEntry is IConfigEntryTagsData)
                tags = ((IConfigEntryTagsData)CFMEntry).Tags;

            if (CFMEntry is IConfigEntryAcceptableValueListData<TVal>)
                accList = ((IConfigEntryAcceptableValueListData<TVal>)CFMEntry).AcceptableList;

            if (CFMEntry is IConfigEntryAcceptableValueRangeData<TVal>)
                accRange = ((IConfigEntryAcceptableValueRangeData<TVal>)CFMEntry).AcceptableRange;

            ConfigDescription desc = null;

            if (accList != null)
                desc = new ConfigDescription(CFMEntry.DescriptionString, accList, tags);
            else if (accRange != null)
                desc = new ConfigDescription(CFMEntry.DescriptionString, accRange, tags);
            else
                desc = new ConfigDescription(CFMEntry.DescriptionString, null, tags);

            CFMEntry.RawEntry = config.Bind<TVal>(
                section, 
                CFMEntry.DefaultKey,
                CFMEntry.DefaultValue,
                desc
                );
        }

        public static void BindCFModel<T>(this T CFModel) where T : IConfigModelData
        {
            throw new NotImplementedException();
        }
    }
}
