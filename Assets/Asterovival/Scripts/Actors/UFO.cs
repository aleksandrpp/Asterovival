using AK.Asterovival.Parts;

namespace AK.Asterovival.Actors
{
    public struct UFO : ITransform, IDynamics
    {
        public Transform Transform { get; set; }
        public Dynamics Dynamics { get; set; }
    }
}
