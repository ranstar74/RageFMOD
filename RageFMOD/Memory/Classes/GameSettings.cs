using FusionLibrary.Extensions;
using RageAudio.Memory.Enums;
using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory.Classes
{
	/// <summary>
	/// Contains information about various game settings from pause menu.
	/// </summary>
	internal static class GameSettings
	{
		/// <summary>
		/// Whether game is paused this frame or not.
		/// </summary>
		public static bool IsGamePaused
		{
			get
			{
				return Convert.ToBoolean(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.IsGamePausedOffset));
			}
		}

		/// <summary>
		/// Global sound volume in range of 0.0 - 1.0
		/// </summary>
		/// <remarks>
		/// Game stores it as one byte, so theres only 10 volume levels.
		/// </remarks>
		public static float SoundVolume
        {
            get
            {
				if(NativeMemory.GameSettingsAddr == IntPtr.Zero)
                {
					return 0f;
				}

				return Convert.ToSingle(Marshal.ReadByte(NativeMemory.GameSettingsAddr + NativeMemory.SoundVolumeOffset)) / 10;
			}
            set
            {
				if (NativeMemory.GameSettingsAddr == IntPtr.Zero)
				{
					return;
				}

				value = value.Clamp(0, 1);

				Marshal.WriteByte(NativeMemory.GameSettingsAddr + NativeMemory.SoundVolumeOffset, Convert.ToByte(value * 10));
			}
		}

		/// <summary>
		/// Global music volume in range of 0.0 - 1.0
		/// </summary>
		/// <remarks>
		/// Game stores it as one byte, so theres only 10 volume levels.
		/// </remarks>
		public static float MusicVolume
		{
			get
			{
				return Convert.ToSingle(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.MusicVolumeOffset)) / 10;
			}
			set
			{
				if (NativeMemory.GameSettingsAddr == IntPtr.Zero)
				{
					return;
				}

				value = value.Clamp(0, 1);

				Marshal.WriteByte(NativeMemory.GameSettingsAddr + NativeMemory.MusicVolumeOffset, Convert.ToByte(value * 10));
			}
		}

		/// <summary>
		/// Global boost of dialog volume in range of 0.0 - 1.0
		/// </summary>
		/// <remarks>
		/// Game stores it as one byte, so theres only 10 volume levels.
		/// </remarks>
		public static float DialogBoost
        {
            get
            {
				return Convert.ToSingle(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.DialogBoostOffset) / 10);
            }
			set
			{
				if (NativeMemory.GameSettingsAddr == IntPtr.Zero)
				{
					return;
				}

				value = value.Clamp(0, 1);

				Marshal.WriteByte(NativeMemory.GameSettingsAddr + NativeMemory.DialogBoostOffset, Convert.ToByte(value * 10));
			}
		}

		/// <summary>
		/// Format of game audio output.
		/// </summary>
		public static SoundOutput SoundOutputMode
        {
			get
			{
				return (SoundOutput) NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.SoundOutputModeOffset);
			}
		}

		public static RadioStation RadioStation
        {
			get
			{
				return (RadioStation)NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.RadioStationOffset);
			}
		}

		public static TransportCameraHeight TransportCameraHeight
        {
			get
			{
				return (TransportCameraHeight)NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.TransportCameraHeightOffset);
			}
		}

		public static Radar Radar
        {
			get
			{
				return (Radar)NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.RadarOffset);
			}
		}

		public static bool IsUserInterfaceVisible
        {
			get
			{
				return Convert.ToBoolean(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.IsUserInterfaceVisibleOffset));
			}
		}
		public static Crosshair CrosshairTarget
        {
			get
			{
				return (Crosshair)NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.CrosshairTargetOffset);
			}
		}
		public static float SimpleReticuleSize
        {
			get
			{
				return Convert.ToSingle(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.SimpleReticuleSizeOffset) / 10);
			}
			set
			{
				if (NativeMemory.GameSettingsAddr == IntPtr.Zero)
				{
					return;
				}

				value = value.Clamp(0, 1);

				Marshal.WriteByte(NativeMemory.GameSettingsAddr + NativeMemory.SimpleReticuleSizeOffset, Convert.ToByte(value * 10));
			}
		}


		public static bool IsGpsRouteVisible
        {
			get
			{
				return Convert.ToBoolean(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.IsGpsRouteVisibleOffset));
			}
		}

		public static float SafezoneSize
        {
			get
			{
				return Convert.ToSingle(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.SafezoneSizeOffset) / 10);
			}
			set
			{
				if (NativeMemory.GameSettingsAddr == IntPtr.Zero)
				{
					return;
				}

				value = value.Clamp(0, 1);

				Marshal.WriteByte(NativeMemory.GameSettingsAddr + NativeMemory.SafezoneSizeOffset, Convert.ToByte(value * 10));
			}
		}

		public static bool MuteSoundOnFocusLost
        {
			get
			{
				return Convert.ToBoolean(NativeMemory.Get<byte>(NativeMemory.GameSettingsAddr, NativeMemory.MuteSoundOnFocusLostOffset));
			}
		}
	}
}
