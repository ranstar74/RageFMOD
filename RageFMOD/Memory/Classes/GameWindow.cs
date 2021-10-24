using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory.Classes
{
    /// <summary>
    /// Contains various information about game window.
    /// </summary>
    internal static class GameWindow
    {
        /// <summary>
        /// Returns True if game window is focused, otherwise False.
        /// </summary>
        public static bool IsWindowFocused
        {
            get
            {
                // FIXME: TEMP FIX
                return true;

                if (NativeMemory.IsWindowFocusedAddr == IntPtr.Zero)
                {
                    return false;
                }

                return Convert.ToBoolean(Marshal.ReadByte(NativeMemory.IsWindowFocusedAddr));
            }
        }
    }
}
