using GTA.Math;
using RageAudio.Helpers;
using RageAudio.Memory.Classes;
using RageAudio.Memory.Interfaces;
using RageAudio.Memory.Tools;
using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory
{
    /// <summary>
    /// Contains various game memory addresses.
    /// </summary>
	/// <remarks>
	/// VER_1_0_2372_0_STEAM
	/// </remarks>
    internal unsafe static class NativeMemory
    {
        // Scan addresses

        public static readonly IntPtr GameSettingsAddr;
        public static readonly IntPtr IsGameWindowFocusedAddr;
        public static readonly IntPtr CViewportGameAddr;

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
        public const int ReverbOffset = 0x38;
        public const int EchoOffset = 0x3C;
        public const int EnvironmentReverbOffset = 0xB8;

        public static CViewportGame* CViewportGame;

        /// <summary>
        /// Scans game memory.
        /// </summary>
        static NativeMemory()
        {
            unsafe
            {
                Pattern pattern;
                IntPtr address;
                int offset;

                // GameSettings

                pattern = new Pattern(
                    "\x4c\x8d\x15\x00\x00\x00\x00\x48\x63\xc1\x45\x8b\x04\x82",
                    "xxx????xxxxxxx");
                address = pattern.Get(3);
                offset = *(int*)address;
                GameSettingsAddr = address + offset + 4 + 0x1C;

                LogHelper.Log(GameSettingsAddr, nameof(GameSettingsAddr));

                // IsGameWindowFocused
                pattern = new Pattern(
                    "\x88\x05\x00\x00\x00\x00\xf6\xd8\x1a\xc0",
                    "xx????xxxx");
                address = pattern.Get(2);
                offset = *(int*)address;
                IsGameWindowFocusedAddr = address + offset + 4;

                LogHelper.Log(IsGameWindowFocusedAddr, nameof(IsGameWindowFocusedAddr));

                // ViewportGame

                pattern = new Pattern(
                    "\x48\x8b\x0d\x00\x00\x00\x00\x48\x83\xc1\x00\xf3\x0f\x10\x9b\x00\x00\x00\x00\xf3\x0f\x10\x93",
                    "xxx????xxx?xxxx????xxxx");
                address = pattern.Get(3);
                offset = *(int*)address;
                CViewportGameAddr = address + offset + 4;

                LogHelper.Log(CViewportGameAddr, nameof(CViewportGameAddr));

                if (CViewportGameAddr != IntPtr.Zero)
                {
                    CViewportGame = *(CViewportGame**)CViewportGameAddr;
                }
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
        public static unsafe T Get<T>(IntPtr baseAddr, int offset = 0)
        {
            IntPtr addr = baseAddr + offset;
            Type tType = typeof(T);
            T defaultValue;

            // TODO: Move to extension
            if(tType.GetInterface(nameof(INativeMemory)) != null)
            {
                defaultValue = (T) Activator.CreateInstance(tType, *(IntPtr*)addr.ToPointer());
            }
            else
            {
                defaultValue = Activator.CreateInstance<T>();
            }

            if (baseAddr == IntPtr.Zero)
                return defaultValue;

            if (baseAddr == null)
                return defaultValue;

            // Addresses below 0x1000 most likely are NullPtr
            if (baseAddr.ToInt64() <= 0x1000)
                return defaultValue;

            if (tType == typeof(byte))
                return (T)Convert.ChangeType(Marshal.ReadByte(addr), typeof(byte));
            else if (tType == typeof(Int16))
                return (T)Convert.ChangeType(Marshal.ReadInt16(addr), typeof(Int16));
            else if (tType == typeof(Int32))
                return (T)Convert.ChangeType(Marshal.ReadInt32(addr), typeof(Int32));
            else if (tType == typeof(Int64))
                return (T)Convert.ChangeType(Marshal.ReadInt64(addr), typeof(Int64));
            else if (tType == typeof(float))
                return (T)Convert.ChangeType(*(float*)addr, tType);
            else if(tType == typeof(bool))
                return (T)Convert.ChangeType(*(bool*)addr, tType);
            else if(tType == typeof(Vector3))
            {
                float x = Get<float>(baseAddr);
                float y = Get<float>(baseAddr, 0x4);
                float z = Get<float>(baseAddr, 0x8);

                return (T)Convert.ChangeType(new Vector3(x, y, z), typeof(Vector3));
            }
            else return defaultValue;
        }
    }
}
