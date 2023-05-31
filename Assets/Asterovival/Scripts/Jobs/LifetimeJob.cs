using AK.Asterovival.Parts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AK.Asterovival
{
    [BurstCompile]
    public struct LifetimeJob<T> : IJob where T : unmanaged, ILifetime
    {
        public NativeList<T> Actors;
        [ReadOnly] public float DeltaTime;

        public void Execute()
        {
            for (int i = 0; i < Actors.Length; i++)
            {
                var a = Actors[i];
                var l = a.Lifetime;

                if (!l.TickAlive(DeltaTime))
                {
                    Actors.RemoveAtSwapBack(i--);
                    continue;
                }

                a.Lifetime = l;
                Actors[i] = a;
            }
        }
    }
}
