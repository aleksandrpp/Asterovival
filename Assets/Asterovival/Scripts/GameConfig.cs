using UnityEngine;

namespace AK.Asterovival
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "AK.Asterovival/GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        public Bounds
            Bounds = new(new Vector3(0, 0, 0), new Vector3(30, 1, 30));

        public InstanceConfig
            Asteroid,
            UFO,
            Projectile,
            Ship;

        public float
            TimeToWave = 10;

        public int
            AsteroidsPerWave = 10,
            UfosPerWave = 3;
    }
}
