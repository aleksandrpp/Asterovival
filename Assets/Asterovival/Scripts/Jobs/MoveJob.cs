using AK.Asterovival.Parts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Transform = AK.Asterovival.Parts.Transform;

namespace AK.Asterovival
{
    [BurstCompile]
    public struct MoveJob<T> : IJobParallelFor where T : struct, ITransform, IDynamics
    {
        private const float SpeedMlp = 10;
        private const float TargetSpeedMlp = 3;

        public NativeArray<T> Actors;

        [ReadOnly] public float3 Target;
        [ReadOnly] public bool Targeted;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public Bounds Bounds;

        public void Execute(int index)
        {
            var a = Actors[index];
            var t = a.Transform;
            var d = a.Dynamics;

            if (!Bounds.Contains(t.Position))
            {
                var pos = new float3(-t.Position.x, 0, -t.Position.z);
                var lpo = new float3(-d.LastPosition.x, 0, -d.LastPosition.z);

                a.Dynamics = new Dynamics()
                {
                    LastPosition = pos,
                    Impulse = d.Impulse
                };

                a.Transform = new Transform()
                {
                    Position = lpo,
                    Rotation = t.Rotation,
                    Scale = t.Scale
                };

                Actors[index] = a;
                return;
            }

            var impulse = d.Impulse;
            impulse.y = 0;

            if (Targeted)
            {
                impulse = math.normalizesafe(Target - t.Position, impulse) * math.length(impulse);
            }

            var position = t.Position + impulse * (Targeted ? TargetSpeedMlp : SpeedMlp) * DeltaTime;

            a.Dynamics = new Dynamics()
            {
                LastPosition = t.Position,
                Impulse = impulse
            };

            a.Transform = new Transform()
            {
                Position = position,
                Rotation = quaternion.LookRotation(math.normalize(position - t.Position), math.up()),
                Scale = t.Scale
            };

            Actors[index] = a;
        }
    }
}
