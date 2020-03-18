using Unity.Entities;
using VRDF.Core.Inputs;

namespace VRDF.Core.VRInteractions
{
    public struct ControllersInteractionType : IComponentData
    {
        public EControllerInteractionType InteractionType;

        public bool HasClickInteraction;
        public bool HasTouchInteraction;
    }
}
