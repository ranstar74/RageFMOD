using GTA;
using RageAudio.Memory.Classes.Audio;
using RageAudio.Memory.Interfaces;
using System;

namespace RageAudio.Memory.Classes.Entity
{
    /// <summary>
    /// Represents CVehicle structure.
    /// </summary>
    public class CVehicle : INativeMemory
	{
		/// <summary>
		/// Gets the memory address of the <see cref="CVehicle"/>.
		/// </summary>
		public IntPtr MemoryAddress { get; }

		/// <summary>
		/// <see cref="VehicleAudio"/> instance of the <see cref="CVehicle"/>.
		/// </summary>
		public readonly VehicleAudio VehicleAudio;

		/// <summary>
		/// Creates a new instance of <see cref="CVehicle"/> with given <see cref="Vehicle"/> memory address.
		/// </summary>
		/// <param name="memoryAddress">Address of the vehicle.</param>
		public CVehicle(IntPtr memoryAddress)
        {
			MemoryAddress = memoryAddress;
			VehicleAudio = NativeMemory.Get<VehicleAudio>(MemoryAddress, NativeMemory.VehicleAudioOffset);
        }
    }
}
