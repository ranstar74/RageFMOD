using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RageAudio.Memory.Classes
{
    /// <summary>
    /// Provides various game world information.
    /// </summary>
    internal static class GameWorld
    {
        /// <summary>
        /// Whether random trains are enabled or not.
        /// </summary>
        public static bool AreRandomTrainsOn
        {
            get
            {
                return Convert.ToBoolean(NativeMemory.Get<byte>(NativeMemory.AreRandomTrainsOn));
            }
            set
            {
                Marshal.WriteByte(NativeMemory.AreRandomTrainsOn, Convert.ToByte(value));
            }
        }
    }
}
