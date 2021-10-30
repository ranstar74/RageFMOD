using RageAudio.Memory.Interfaces;
using System;

namespace RageAudio.Memory.Classes.Audio
{
    /// <summary>
    /// Stores information about environment group such as reverb or echo.
    /// </summary>
    public class EnvironmentGroup : INativeMemory
    {
        /// <summary>
        /// Gets the memory address where the <see cref="EnvironmentGroup"/> is stored in memory.
        /// </summary>
        public IntPtr MemoryAddress { get; }

        /// <summary>
        /// Audio reverb amount.
        /// </summary>
        public float Reverb
        {
            get => NativeMemory.Get<float>(MemoryAddress, NativeMemory.ReverbOffset);
        }

        /// <summary>
        /// Environment audio reverb amount.
        /// </summary>
        public float EnvironmentReverb
        {
            get => NativeMemory.Get<float>(MemoryAddress, NativeMemory.EnvironmentReverbOffset);
        }

        /// <summary>
        /// Echo reverb amount.
        /// </summary>
        public float Echo
        {
            get => NativeMemory.Get<float>(MemoryAddress, NativeMemory.EchoOffset);
        }

        /// <summary>
        /// Creates an <see cref="EnvironmentGroup"/> instance from given pointer.
        /// </summary>
        /// <param name="memoryAddress">Pointer to the <see cref="EnvironmentGroup"/>.</param>
        public EnvironmentGroup(IntPtr memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }
    }
}
