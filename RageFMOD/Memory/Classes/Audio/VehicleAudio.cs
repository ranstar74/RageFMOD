using GTA;
using RageAudio.Memory.Interfaces;
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
    public class VehicleAudio : INativeMemory
    {
        /// <summary>
        /// Gets the memory address of the <see cref="VehicleAudio"/>.
        /// </summary>
        public IntPtr MemoryAddress { get; }

        /// <summary>
        /// <see cref="EnvironmentGroup"/> instance of the <see cref="VehicleAudio"/>.
        /// </summary>
        public readonly EnvironmentGroup EnvironmentGroup;

        /// <summary>
        /// Creates an <see cref="EnvironmentGroup"/> instance from given pointer.
        /// </summary>
        /// <param name="memoryAddress">Pointer to the <see cref="VehicleAudio"/>.</param>
        public VehicleAudio(IntPtr memoryAddress)
        {
            MemoryAddress = memoryAddress;
            EnvironmentGroup = NativeMemory.Get<EnvironmentGroup>(MemoryAddress, NativeMemory.EnvironmentGroupOffset);
        }
    }
}
