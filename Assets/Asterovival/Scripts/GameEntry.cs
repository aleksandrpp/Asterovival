using AK.Asterovival.Actors;
using AK.Asterovival.Parts;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Transform = AK.Asterovival.Parts.Transform;

namespace AK.Asterovival
{
    public class GameEntry : MonoBehaviour
    {
        private const int BatchCount = 32;

        [SerializeField] private GameInput _input;
        [SerializeField] private GameConfig _gameConfig;

        private NativeReference<float> _waveTimer;
        private NativeReference<Ship> _ship;
        private NativeList<Projectile> _projectiles;
        private NativeList<Asteroid> _astroids;
        private NativeList<UFO> _ufos;

        public string ShipDebug => _ship.Value.Debug();
        public string Score => _ship.Value.Score.ToString();
        public bool GameOver => _ship.Value.Lives <= 0;

        private void OnEnable()
        {
            _projectiles = new NativeList<Projectile>(Allocator.Persistent);
            _astroids = new NativeList<Asteroid>(Allocator.Persistent);
            _ufos = new NativeList<UFO>(Allocator.Persistent);

            _ship = new NativeReference<Ship>(Allocator.Persistent)
            {
                Value = new Ship()
                {
                    Transform = new Transform()
                    {
                        Position = float3.zero,
                        Rotation = quaternion.identity,
                        Scale = 1
                    },
                    Dynamics = new Dynamics()
                    {
                        LastPosition = float3.zero,
                        Impulse = 0
                    },
                    Lives = 5,
                    LaserCount = 10
                }
            };

            _waveTimer = new NativeReference<float>(Allocator.Persistent);
        }

        private void Update()
        {
            if (_input.RestartAction.IsPressed())
            {
                OnDisable();
                OnEnable();
            }

            if (_ship.Value.Lives <= 0)
            {
                return;
            }

            Simulate();

            Render();
        }

        private void Simulate()
        {
            var wave = new WaveJob()
            {
                WaveTimer = _waveTimer,
                TimeToWave = _gameConfig.TimeToWave,
                Asteroids = _astroids,
                Ufos = _ufos,
                DeltaTime = Time.deltaTime,
                AsteroidsCount = _gameConfig.AsteroidsPerWave,
                UfosCount = _gameConfig.UfosPerWave,
                Bounds = _gameConfig.Bounds
            };

            wave.Schedule().Complete();

            var asteroid = new MoveJob<Asteroid>()
            {
                Actors = _astroids.AsArray(),
                DeltaTime = Time.deltaTime,
                Bounds = _gameConfig.Bounds
            };

            var ufo = new MoveJob<UFO>()
            {
                Actors = _ufos.AsArray(),
                DeltaTime = Time.deltaTime,
                Target = _ship.Value.Transform.Position,
                Targeted = true,
                Bounds = _gameConfig.Bounds
            };

            var projectile = new MoveJob<Projectile>()
            {
                Actors = _projectiles.AsArray(),
                DeltaTime = Time.deltaTime,
                Bounds = _gameConfig.Bounds
            };

            var ship = new ShipJob()
            {
                Ship = _ship,
                DeltaTime = Time.deltaTime,
                Left = _input.LeftAction.IsPressed(),
                Right = _input.RightAction.IsPressed(),
                Thrust = _input.ThrustAction.IsPressed(),
                Fire = _input.FireAction.IsPressed(),
                Laser = _input.LaserAction.IsPressed(),
                Projectiles = _projectiles,
                Bounds = _gameConfig.Bounds
            };

            var lifetime = new LifetimeJob<Projectile>
            {
                Actors = _projectiles,
                DeltaTime = Time.deltaTime
            };

            var hit = new HitJob()
            {
                Asteroids = _astroids,
                Projectiles = _projectiles,
                Ship = _ship,
                Ufos = _ufos
            };

            var jh = asteroid.Schedule(_astroids.Length, BatchCount);
            jh = ufo.Schedule(_ufos.Length, BatchCount, jh);
            jh = projectile.Schedule(_projectiles.Length, BatchCount, jh);
            jh = ship.Schedule(jh);
            jh = lifetime.Schedule(jh);
            jh = hit.Schedule(jh);
            jh.Complete();
        }

        private void Render()
        {
            using var ship = new NativeArray<Ship>(new Ship[] { _ship.Value }, Allocator.TempJob);
            using var asteroids = new NativeArray<Matrix4x4>(_astroids.Length, Allocator.TempJob);
            using var ufos = new NativeArray<Matrix4x4>(_ufos.Length, Allocator.TempJob);
            using var ships = new NativeArray<Matrix4x4>(1, Allocator.TempJob);
            using var projectiles = new NativeArray<Matrix4x4>(_projectiles.Length, Allocator.TempJob);

            var asteroidsMatrices = new MatrixJob<Asteroid>()
            {
                Actors = _astroids.AsArray(),
                Matrices = asteroids
            };

            var ufosMatrices = new MatrixJob<UFO>()
            {
                Actors = _ufos.AsArray(),
                Matrices = ufos
            };

            var shipsMatrices = new MatrixJob<Ship>()
            {
                Actors = ship,
                Matrices = ships
            };

            var projectilesMatrices = new MatrixJob<Projectile>()
            {
                Actors = _projectiles.AsArray(),
                Matrices = projectiles
            };

            var jh = asteroidsMatrices.Schedule(_astroids.Length, BatchCount);
            jh = ufosMatrices.Schedule(_ufos.Length, BatchCount, jh);
            jh = shipsMatrices.Schedule(ship.Length, BatchCount, jh);
            jh = projectilesMatrices.Schedule(_projectiles.Length, BatchCount, jh);
            jh.Complete();

            Graphics.DrawMeshInstanced(_gameConfig.Asteroid.Mesh, 0, _gameConfig.Asteroid.Material, asteroids.ToArray());
            Graphics.DrawMeshInstanced(_gameConfig.UFO.Mesh, 0, _gameConfig.UFO.Material, ufos.ToArray());
            Graphics.DrawMeshInstanced(_gameConfig.Ship.Mesh, 0, _ship.Value.ImmortalTimer > 0 ? _gameConfig.Ship.SubMaterial : _gameConfig.Ship.Material, ships.ToArray());
            Graphics.DrawMeshInstanced(_gameConfig.Projectile.Mesh, 0, _gameConfig.Projectile.Material, projectiles.ToArray());
        }

        private void OnDisable()
        {
            _ship.Dispose();
            _waveTimer.Dispose();
            _projectiles.Dispose();
            _astroids.Dispose();
            _ufos.Dispose();
        }
    }
}
