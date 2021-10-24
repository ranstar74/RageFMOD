using RageAudio.Helpers;
using RageAudio.Memory.Tools;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RageAudio.Memory
{
    /// <summary>
    /// Contains various game memory addresses.
    /// </summary>
	/// <remarks>
	/// VER_1_0_2372_0_STEAM
	/// </remarks>
    internal static class NativeMemory
    {
        public static readonly IntPtr AreRandomTrainsOn;

        public static readonly IntPtr IsGamePaused;

        public static readonly IntPtr HudInfoAddr;
      
        public const int IsPlayerSwitchInProgressOffset = 0xA;
        public const int IsPlayerSelectorOpenedOffset = 0x29;

        public static readonly IntPtr IsWindowFocusedAddr;

        public static readonly IntPtr GameSettingsAddr;

        // GameSettings -> Audio
        public const int SoundVolumeOffset = 0x0;
        public const int MusicVolumeOffset = 0x4;
        public const int DialogBoostOffset = 0x8c;
        public const int SoundOutputModeOffset = 0xC;
        public const int RadioStationOffset = 0x14;
        public const int FrontSurroundSpeakersModeOffset = 0x94;
        public const int RearSurroundSpeakersModeOffset = 0x98;
        public const int SelfRadioModeOffset = 0x280;
        public const int AutoMusicScanOffset = 0x284;
        public const int MuteSoundOnFocusLostOffset = 0x288;
        public const int IsGamePausedOffset = 0x2B9;

        // GameSettings -> Camera
        public const int TransportCameraHeightOffset = 0x24;

        // GameSettings -> Video
        public const int RadarOffset = 0x54;
        public const int IsUserInterfaceVisibleOffset = 0x58;
        public const int CrosshairTargetOffset = 0x5C;
        public const int SimpleReticuleSizeOffset = 0x60;
        public const int IsGpsRouteVisibleOffset = 0x64;
        public const int SafezoneSizeOffset = 0x68;

        // Vehicle
        public const int VehicleAudioOffset = 0x970;

        // Audio
        public const int EnvironmentGroupOffset = 0x38;

        /// <summary>
        /// Scans game memory.
        /// </summary>
        static NativeMemory()
        {
            unsafe
            {
                Pattern pattern;

                AreRandomTrainsOn = IntPtr.Zero;

                //pattern = new Pattern(
                //     "\x03\x00\x00\x00\x01\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x90",
                //     "xxxxxxxxxxxxxxxxxxxxx");
                // HudInfoAddr = (IntPtr)pattern.Get().ToPointer();
                // LogHelper.Log(HudInfoAddr, nameof(HudInfoAddr));

                // ---
                HudInfoAddr = IntPtr.Zero;
                //pattern = new Pattern(
                //    "\x80\x69\x00\x00\xf7\x7f\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x2e\x3f\x41\x56\x53\x68\x00\x00\x00\x00\x56",
                //    "xx??xx?xxxxxxxxxxxxxxx????x");
                //IsWindowFocusedAddr = (IntPtr)pattern.Get().ToPointer() + 0xB3;
                //LogHelper.Log(IsWindowFocusedAddr, nameof(IsWindowFocusedAddr));

                //pattern = new Pattern("\x18\x40\x00\x55\xf7\x7f\x00\x00\x60",
                //    "xx?xxx?xx");
                //IsWindowFocusedAddr = (IntPtr)pattern.Get().ToPointer() - 0x4D5;
                //LogHelper.Log(IsWindowFocusedAddr, nameof(IsWindowFocusedAddr));
                IsWindowFocusedAddr = IntPtr.Zero;

                // ---

                pattern = new Pattern(
                    "\x00\x00\x00\x00\x00\x00\x00\x00\x01\x00\x00\x00\x03\x00\x00\x00\x00\x00\x00\x00\x05",
                    "xxxxxxxxxxxxxxxxxxxxx");
                GameSettingsAddr = (IntPtr)pattern.Get().ToPointer() + 0x1C;
                LogHelper.Log(GameSettingsAddr, nameof(GameSettingsAddr));
            }
        }

        /// <summary>
        /// Safe get method that gets value from base address and offset.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="baseAddr">Base address to get value from.</param>
        /// <param name="offset">Memory offset from base address.</param>
        /// <returns>
        /// Value from given pointer. 
        /// In case if base address is nullptr, it returns default value.
        /// </returns>
        public static T Get<T>(IntPtr baseAddr, int offset = 0)
        {
            T defaultValue = Activator.CreateInstance<T>();
            if (baseAddr == IntPtr.Zero || baseAddr == null)
                return defaultValue;

            IntPtr addr = baseAddr + offset;

            Type tType = typeof(T);

            unsafe
            {
                if (tType == typeof(byte))
                    return (T)Convert.ChangeType(Marshal.ReadByte(addr), typeof(byte));
                else if (tType == typeof(Int16))
                    return (T)Convert.ChangeType(Marshal.ReadInt16(addr), typeof(Int16));
                else if (tType == typeof(Int32))
                    return (T)Convert.ChangeType(Marshal.ReadInt32(addr), typeof(Int32));
                else if (tType == typeof(Int64))
                    return (T)Convert.ChangeType(Marshal.ReadInt64(addr), typeof(Int64));
                else return defaultValue;
            }
        }
    }
}
