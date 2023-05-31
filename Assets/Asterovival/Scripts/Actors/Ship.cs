using AK.Asterovival.Parts;
using Unity.Mathematics;

namespace AK.Asterovival.Actors
{
    public struct Ship : ITransform, IDynamics
    {
        public Transform Transform { get; set; }
        public Dynamics Dynamics { get; set; }

        public float LaserTimer;
        public int LaserCount;
        public float FireTimer;
        public float Angle;
        public int Lives;
        public float ImmortalTimer;
        public int Score;

        public string Debug()
        {
            string LaserString = (LaserCount > 0) ? LaserTimer.ToString("F1") : "";

            return 
                $"Position: X({Transform.Position.x:F1}) Y({Transform.Position.z:F1})\n" +
                $"Angle: {math.abs(Angle % 180):F0}\n" +
                $"Speed: {math.length(Dynamics.Impulse):F1}\n" +
                $"Lives: {Lives}\n" +
                $"Laser: {LaserCount} {LaserString}\n" +
                $"Score: {Score}";
        }
    }
}
