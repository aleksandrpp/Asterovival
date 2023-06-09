﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace AK.Asterovival.Parts
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Transform
    {
        public float3 Position;
        public quaternion Rotation;
        public float Scale;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4 СomposeMatrix()
        {
            return float4x4.TRS(Position, Rotation, new float3(Scale));
        }
    }
}
