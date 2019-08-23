using Unity.Mathematics;

namespace VRSF.MoveAround.Fly
{
    public struct FlyBoundaries : Unity.Entities.IComponentData
    {
        public bool UseBoundaries;

        /// <summary>
        /// The minimun local position at which the user can go if UseHorizontalBoundaries is at true.
        /// </summary>
        public float3 MinAvatarPosition;

        /// <summary>
        /// The maximum local position at which the user can go if UseHorizontalBoundaries is at true.
        /// </summary>
        public float3 MaxAvatarPosition;
    }
}