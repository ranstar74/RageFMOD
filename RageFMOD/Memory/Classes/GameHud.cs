using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory.Classes
{
    /// <summary>
    /// Provides various HUD related information.
    /// </summary>
    internal static class GameHud
    {
        /// <summary>
        /// Whether player switch is in progress this frame or not.
        /// </summary>
        public static bool IsPlayerSwitching
        {
            get
            {
                if(NativeMemory.HudInfoAddr == IntPtr.Zero)
                {
                    return false;
                }

                return Convert.ToBoolean(
                    Marshal.ReadByte(NativeMemory.HudInfoAddr + NativeMemory.IsPlayerSwitchInProgressOffset));
            }
        }

        /// <summary>
        /// Whether player selector is opened this frame or not.
        /// </summary>
        public static bool IsPlayerSelectorOpened
        {
            get
            {
                if (NativeMemory.HudInfoAddr == IntPtr.Zero)
                {
                    return false;
                }

                return Convert.ToBoolean(
                    Marshal.ReadByte(NativeMemory.HudInfoAddr + NativeMemory.IsPlayerSelectorOpenedOffset));
            }
        }
    }
}
