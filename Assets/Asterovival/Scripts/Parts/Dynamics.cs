using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace AK.Asterovival.Parts
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Dynamics
    {
        public float3 LastPosition;
        public float3 Impulse;
    }
}
