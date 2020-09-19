using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx.Configuration;

namespace BepInEx.Extensions.Configuration
{
    public interface IConfigEntryAcceptableValueListData<T> where T : System.IEquatable<T>
    {
        AcceptableValueList<T> AcceptableList { get; }
    }
}
