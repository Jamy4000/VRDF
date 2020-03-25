using Unity.Entities;

namespace VRDF.MoveAround.Teleport
{
    /// <summary>
    /// Tag used in the Curve teleporter systems to calculate and render a parabol
    /// </summary>
    public struct ParabolPointParent : ISharedComponentData
    {
        public int TeleporterEntityIndex;
    }
}
