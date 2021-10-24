using GTA;
using RageAudio.Memory.Classes.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RageAudio.Memory.Extensions
{
    /// <summary>
    /// Memory extensions methods for <see cref="Vehicle"/>.
    /// </summary>
    public static class VehicleMemoryExtensions
    {
        /// <summary>
        /// Gets <see cref="VehicleAudio"/> instance of this <see cref="Vehicle"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns><see cref="VehicleAudio"/> instance.</returns>
        public static VehicleAudio GetAudio(this Vehicle context)
        {
            return new VehicleAudio(context);
        }
    }
}
