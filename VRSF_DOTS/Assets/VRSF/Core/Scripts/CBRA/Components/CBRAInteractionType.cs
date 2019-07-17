using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    public struct CBRAInteractionType : IComponentData
    {
        public EControllerInteractionType InteractionType;
    }
}
