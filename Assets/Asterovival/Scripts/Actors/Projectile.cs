using AK.Asterovival.Parts;

namespace AK.Asterovival.Actors
{
    public struct Projectile : ITransform, IDynamics, ILifetime
    {
        public Transform Transform { get; set; }
        public Dynamics Dynamics { get; set; }
        public Lifetime Lifetime { get; set; }
    }
}
