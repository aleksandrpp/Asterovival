using AK.Asterovival.Parts;

namespace AK.Asterovival.Actors
{
    public struct Asteroid : ITransform, IDynamics
    {
        public Transform Transform { get; set; }
        public Dynamics Dynamics { get; set; }
    }
}
