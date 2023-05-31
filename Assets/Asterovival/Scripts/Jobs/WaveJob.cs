using AK.Asterovival.Actors;
using AK.Asterovival.Parts;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Transform = AK.Asterovival.Parts.Transform;

namespace AK.Asterovival
{
    public struct WaveJob : IJob
    {
        public NativeReference<float> WaveTimer;
        [ReadOnly] public float TimeToWave;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int AsteroidsCount;
        [ReadOnly] public int UfosCount;
        [ReadOnly] public Bounds Bounds;

        public NativeList<Asteroid> Asteroids;
        public NativeList<UFO> Ufos;

        public void Execute()
        {
            if ((WaveTimer.Value -= DeltaTime) > 0) return;
            WaveTimer.Value = TimeToWave;

            var rnd = new Random(1);

            for (int i = 0; i < AsteroidsCount; i++)
            {
               
                var p = rnd.NextFloat3(Bounds.min, Bounds.max);
                p.y = 0;

                Asteroids.Add(new Asteroid()
                {
                    Transform = new Transform()
                    {
                        Position = p,
                        Rotation = quaternion.identity,
                        Scale = 3
                    },
                    Dynamics = new Dynamics()
                    {
                        Impulse = rnd.NextFloat3(),
                        LastPosition = p
                    }
                });
            }

            for (int i = 0; i < UfosCount; i++)
            {
                var p = rnd.NextFloat3(Bounds.min, Bounds.max);
                p.y = 0;

                Ufos.Add(new UFO()
                {
                    Transform = new Transform()
                    {
                        Position = p,
                        Rotation = quaternion.identity,
                        Scale = 1
                    },
                    Dynamics = new Dynamics()
                    {
                        Impulse = rnd.NextFloat3(),
                        LastPosition = p
                    }
                });
            }
        }
    }


}
