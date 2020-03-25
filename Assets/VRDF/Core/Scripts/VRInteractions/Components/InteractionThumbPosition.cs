using Unity.Entities;
using VRDF.Core.Inputs;

namespace VRDF.Core.VRInteractions
{
    public struct InteractionThumbPosition : IComponentData
    {
        public EThumbPosition TouchThumbPosition;
        public float IsTouchingThreshold;

        public EThumbPosition ClickThumbPosition;
        public float IsClickingThreshold;
    }
}
