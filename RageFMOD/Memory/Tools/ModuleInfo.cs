using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RageAudio.Memory.Tools
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ModuleInfo
    {
        public IntPtr lpBaseOfDll;
        public uint SizeOfImage;
        public IntPtr EntryPoint;
    }
}
