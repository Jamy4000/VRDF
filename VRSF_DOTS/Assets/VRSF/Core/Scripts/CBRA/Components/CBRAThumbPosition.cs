using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    public struct CBRAThumbPosition : IComponentData
    {
        public EThumbPosition TouchThumbPosition;
        public float IsTouchingThreshold;

        public EThumbPosition ClickThumbPosition;
        public float IsClickingThreshold;
    }
}
