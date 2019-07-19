using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolPoints : ISharedComponentData
    {
        public NativeList<float3> ParabolaPoints;
    }
}
