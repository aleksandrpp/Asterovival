using AK.Asterovival.Actors;
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
    public struct ShipJob : IJob
    {
        private const float SpeedMlp = 10;
        private const float MaxSpeed = 1.1f;
        private const float RotationSpeedMlp = 150;
        private const float FireImpulseMlp = 2;
        private const float ProjectileDuration = 1.75f;
        private const float LaserTimer = 3;
        private const float FireTimer = .05f;
        private const int LaserCount = 30;

        public NativeReference<Ship> Ship;

        [ReadOnly] public float DeltaTime;
        [ReadOnly] public Bounds Bounds;
        [ReadOnly] public bool Left, Right, Thrust, Fire, Laser;
        [WriteOnly] public NativeList<Projectile> Projectiles;

        public void Execute()
        {
            var a = Ship.Value;
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

                Ship.Value = a;
                return;
            }

            var angle = Ship.Value.Angle;

            angle += Left ? -RotationSpeedMlp * DeltaTime : 0;
            angle += Right ? RotationSpeedMlp * DeltaTime : 0;

            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var direction = rotation * Vector3.forward;

            var impulse = d.Impulse;
            impulse.y = 0;

            if (Thrust)
            {
                impulse += new float3(rotation * Vector3.forward) * DeltaTime;
                impulse = Vector3.ClampMagnitude(impulse, MaxSpeed);
            }

            var position = t.Position + impulse * DeltaTime * SpeedMlp;

            d = new Dynamics()
            {
                Impulse = impulse,
                LastPosition = t.Position
            };

            t = new Transform()
            {
                Position = position,
                Rotation = rotation,
                Scale = t.Scale
            };

            a.Transform = t;
            a.Dynamics = d;
            a.Angle = angle;
            a.ImmortalTimer = math.max(0, a.ImmortalTimer -= DeltaTime);
            a.FireTimer = math.max(0, a.FireTimer -= DeltaTime);
            a.LaserTimer = math.max(0, a.LaserTimer -= DeltaTime);

            if (Fire && a.FireTimer == 0)
            {
                Projectiles.Add(new Projectile()
                {
                    Transform = new Transform()
                    {
                        Position = position,
                        Rotation = quaternion.identity,
                        Scale = 1
                    },
                    Dynamics = new Dynamics()
                    {
                        Impulse = direction * FireImpulseMlp,
                        LastPosition = position
                    },
                    Lifetime = new Lifetime()
                    {
                        Duration = ProjectileDuration
                    }
                });

                a.FireTimer = FireTimer;
            }

            if (a.LaserCount > 0 && Laser && a.LaserTimer == 0)
            {
                var p = position;

                for (int i = 0; i < LaserCount; i++)
                {
                    p += new float3(direction * (i * .01f + 1));

                    Projectiles.Add(new Projectile()
                    {
                        Transform = new Transform()
                        {
                            Position = p,
                            Rotation = quaternion.identity,
                            Scale = 1
                        },
                        Dynamics = new Dynamics()
                        {
                            Impulse = direction * .01f,
                            LastPosition = p
                        },
                        Lifetime = new Lifetime()
                        {
                            Duration = ProjectileDuration
                        }
                    });
                }

                a.LaserCount--;
                a.LaserTimer = LaserTimer;
            }


            Ship.Value = a;
        }
    }
}
