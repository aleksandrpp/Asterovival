using AK.Asterovival.Parts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AK.Asterovival
{
    [BurstCompile]
    public struct MatrixJob<T> : IJobParallelFor where T : struct, ITransform
    {
        [ReadOnly] public NativeArray<T> Actors;
        [WriteOnly] public NativeArray<Matrix4x4> Matrices;

        public void Execute(int index)
        {
            Matrices[index] = Actors[index].Transform.СomposeMatrix();
        }
    }
}
