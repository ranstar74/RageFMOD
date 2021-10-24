using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RageAudio.Memory.Classes.Audio
{
    /// <summary>
    /// Stores information about vehicle audio.
    /// </summary>
    public class VehicleAudio
    {
        /// <summary>
        /// Gets the memory address where the <see cref="VehicleAudio"/> is stored in memory.
        /// </summary>
        public readonly IntPtr MemoryAddress;

        /// <summary>
        /// Owner of this audio.
        /// </summary>
        public readonly Vehicle Vehicle;

        /// <summary>
        /// <see cref="EnvironmentGroup"/> of this audio.
        /// </summary>
        public readonly EnvironmentGroup EnvironmentGroup;

        /// <summary>
        /// Creates an <see cref="VehicleAudio"/> instance for given vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        public VehicleAudio(Vehicle vehicle)
        {
            Vehicle = vehicle;

            MemoryAddress = vehicle.MemoryAddress + NativeMemory.VehicleAudioOffset;
            EnvironmentGroup = new EnvironmentGroup(this);
        }
    }
}
