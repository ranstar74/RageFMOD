using System;

namespace RageAudio.Memory.Tools
{
    /// <summary>
    /// Provides functionality for scanning patterns in game memory.
    /// </summary>
    public sealed class Pattern
    {
        private readonly string _bytes, _mask;

        /// <summary>
        /// Creates a new instance of <see cref="Pattern"/> with given pattern and mask.
        /// </summary>
        /// <param name="bytes">Hex pattern.</param>
        /// <param name="mask">Pattern mask.</param>
        public Pattern(string bytes, string mask)
        {
            _bytes = bytes;
            _mask = mask;
        }

        /// <summary>
        /// Gets pointer to value.
        /// </summary>
        /// <param name="offset">Memory offset.</param>
        /// <returns>Pointer to value.</returns>
        public unsafe IntPtr Get(int offset = 0)
        {
            ModuleInfo module;

            Win32Native.GetModuleInformation(Win32Native.GetCurrentProcess(), Win32Native.GetModuleHandle(null), out module, (uint)sizeof(ModuleInfo));

            var address = module.lpBaseOfDll.ToInt64();

            var end = address + module.SizeOfImage;

            for (; address < end; address++)
            {
                if (BCompare((byte*)(address), _bytes.ToCharArray(), _mask.ToCharArray()))
                {
                    return new IntPtr(address + offset);
                }
            }

            return IntPtr.Zero;
        }

        private unsafe bool BCompare(byte* pData, char[] bMask, char[] szMask)
        {
            for (int i = 0; i < bMask.Length; i++)
            {
                if (szMask[i] == 'x' && pData[i] != bMask[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
