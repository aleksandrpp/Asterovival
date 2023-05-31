using UnityEngine;

namespace AK.Asterovival
{
    [CreateAssetMenu(fileName = "SO_InstanceConfig", menuName = "AK.Asterovival/InstanceConfig")]
    public sealed class InstanceConfig : ScriptableObject
    {
        public Mesh Mesh;
        public Material Material;
        public Material SubMaterial;
    }
}
