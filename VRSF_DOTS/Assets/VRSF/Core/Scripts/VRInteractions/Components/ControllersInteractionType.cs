using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    public struct ControllersInteractionType : IComponentData
    {
        public EControllerInteractionType InteractionType;

        public bool HasClickInteraction;
        public bool HasTouchInteraction;
    }
}
