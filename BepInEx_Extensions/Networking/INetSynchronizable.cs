using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BepInEx.Extensions.Networking
{
    public interface INetSynchronizable
    {
        void Serialize();
    }
}
