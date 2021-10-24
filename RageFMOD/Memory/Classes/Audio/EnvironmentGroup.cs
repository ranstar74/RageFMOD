using System;

namespace RageAudio.Memory.Classes.Audio
{
    /// <summary>
    /// Stores information about environment group such as reverb or echo.
    /// </summary>
    public class EnvironmentGroup
    {
        /// <summary>
        /// Gets the memory address where the <see cref="EnvironmentGroup"/> is stored in memory.
        /// </summary>
        public readonly IntPtr MemoryAddress;

        /// <summary>
        /// Audio reverb amount.
        /// </summary>
        public float Reverb;

        /// <summary>
        /// Echo reverb amount.
        /// </summary>
        public float Echo
        {
            get => NativeMemory.Get<float>(VehicleAudio.Vehicle.MemoryAddress, NativeMemory.EnvironmentGroupOffset);
        }

        /// <summary>
        /// Owner of this environment group.
        /// </summary>
        public readonly VehicleAudio VehicleAudio;

        /// <summary>
        /// Creates an <see cref="EnvironmentGroup"/> instance for given <see cref="VehicleAudio"/>.
        /// </summary>
        /// <param name="vehicleAudio"><see cref="VehicleAudio"/> context.</param>
        public EnvironmentGroup(VehicleAudio vehicleAudio)
        {
            VehicleAudio = vehicleAudio;

            MemoryAddress = vehicleAudio.MemoryAddress + NativeMemory.EnvironmentGroupOffset;
        }
    }
}
