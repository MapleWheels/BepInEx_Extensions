using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigEntryAcceptableValueRangeData<T> where T : System.IComparable
    {
        AcceptableValueRange<T> AcceptableRange { get; }
    }
}
