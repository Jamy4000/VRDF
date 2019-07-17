using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.MoveAround.Rotation
{
    public struct UserRotationInteractionType : IComponentData
    {
        public EControllerInteractionType InteractionType;
    }
}