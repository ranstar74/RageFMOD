using GTA.Math;
using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory.Classes
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
	internal ref struct CViewportGame
	{
		/// <summary>
		/// On Foot - Ped. 
		/// In Vehicle - Vehicle.
		/// Custom Camera - null.
		/// </summary>
		[FieldOffset(0x4E0)]
		public readonly IntPtr EntityTarget;

		[FieldOffset(0x530)]
		public readonly Vector3 RightVector;

		[FieldOffset(0x540)]
		public readonly Vector3 ForwardVector;

		[FieldOffset(0x550)]
		public readonly Vector3 UpVector;

		[FieldOffset(0x560)]
		public readonly Vector3 Position;

		[FieldOffset(0x590)]
		public readonly float Fov;
	}
}
