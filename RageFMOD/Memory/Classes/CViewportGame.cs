using GTA.Math;
using System;
using System.Runtime.InteropServices;

namespace RageAudio.Memory.Classes
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
	internal ref struct CViewportGame
	{
		[FieldOffset(0x100)]
		public readonly Vector3 Position;

		[FieldOffset(0x110)]
		public readonly Vector3 UpVector;

		[FieldOffset(0x130)]
		public readonly Vector3 BackVector;

		/// <summary>
		/// On Foot - Ped. 
		/// In Vehicle - Vehicle.
		/// Custom Camera - null.
		/// </summary>
		[FieldOffset(0x4E0)]
		public readonly IntPtr EntityTarget;
	}
}
