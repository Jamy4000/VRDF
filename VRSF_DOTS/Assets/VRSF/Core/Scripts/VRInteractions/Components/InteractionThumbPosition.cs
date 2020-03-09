using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    public struct InteractionThumbPosition : IComponentData
    {
        public EThumbPosition TouchThumbPosition;
        public float IsTouchingThreshold;

        public EThumbPosition ClickThumbPosition;
        public float IsClickingThreshold;
    }
}
