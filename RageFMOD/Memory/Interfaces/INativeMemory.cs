using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RageAudio.Memory.Interfaces
{
    /// <summary>
    /// Defines a class that initialized from pointer.
    /// </summary>
    internal interface INativeMemory
    {
        IntPtr MemoryAddress { get; }
    }
}
