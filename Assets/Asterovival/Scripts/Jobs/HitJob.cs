using AK.Asterovival.Actors;
using AK.Asterovival.Parts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace AK.Asterovival
{
    [BurstCompile]
    public struct HitJob : IJob
    {
        private const float ImmortalTime = 1;

        public NativeList<Projectile> Projectiles;
        public NativeList<Asteroid> Asteroids;
        public NativeList<UFO> Ufos;
        public NativeReference<Ship> Ship;

        public void Execute()
        {
            var rnd = new Random(1);
            var ship = Ship.Value;

            for (int i = 0; i < Asteroids.Length; i++)
            {
                var at = Asteroids[i].Transform;
                var ad = Asteroids[i].Dynamics;

                if (ship.ImmortalTimer == 0)
                {
                    if (math.distance(Ship.Value.Transform.Position, at.Position) < at.Scale)
                    {
                        ship.Lives--;
                        ship.ImmortalTimer = ImmortalTime;
                    }
                }

                (bool flag, float3 position, float3 lastPosition) removed = (false, at.Position, ad.LastPosition);

                for (int j = 0; j < Projectiles.Length; j++)
                {
                    var pt = Projectiles[j].Transform;

                    if (math.distance(pt.Position, at.Position) >= at.Scale) continue;

                    if (!removed.flag)
                    {
                        Asteroids.RemoveAtSwapBack(i--);
                        ship.Score++;
                    }

                    removed.flag = true;
                    Projectiles.RemoveAtSwapBack(j--);
                }

                if (removed.flag)
                {
                    for (int k = 0; k < at.Scale - 1; k++)
                    {
                        var delta = 0;// rnd.NextFloat3(-1, 1);
                        Asteroids.Add(new Asteroid()
                        {
                            Transform = new Transform()
                            {
                                Position = removed.position + delta,
                                Rotation = quaternion.identity,
                                Scale = at.Scale - 1,
                            },
                            Dynamics = new Dynamics()
                            {
                                Impulse = ad.Impulse,
                                LastPosition = removed.lastPosition + delta
                            }
                        });
                    }
                }
            }

            for (int i = 0; i < Ufos.Length; i++)
            {
                var ut = Ufos[i].Transform;
                var ud = Ufos[i].Dynamics;

                if (ship.ImmortalTimer == 0)
                {
                    if (math.distance(Ship.Value.Transform.Position, ut.Position) < ut.Scale)
                    {
                        ship.Lives--;
                        ship.ImmortalTimer = 1;
                    }
                }

                (bool flag, float3 position, float3 lastPosition) removed = (false, ut.Position, ud.LastPosition);

                for (int j = 0; j < Projectiles.Length; j++)
                {
                    var pt = Projectiles[j].Transform;

                    if (math.distance(pt.Position, ut.Position) >= ut.Scale) continue;

                    if (!removed.flag)
                    {
                        Ufos.RemoveAtSwapBack(i--);
                        ship.Score++;
                    }

                    removed.flag = true;
                    Projectiles.RemoveAtSwapBack(j--);
                }
            }

            ship.Lives = math.max(0, ship.Lives);
            Ship.Value = ship;
        }
    }
}
