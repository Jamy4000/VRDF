using Unity.Entities;

namespace VRDF.MoveAround.Teleport
{
    public struct TeleportNavMesh : IComponentData
    {
        public int QueryTriggerInteraction;
        public bool IgnoreSlopedSurfaces;
        public float SampleRadius;
        public float SphereCastRadius;
        public int NavAreaMask;
    }
}