using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Handle the Click event in VR. Basically link the Raycast system and the Input System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the ObjectWasClickedEvent
    /// </summary>
    public class PointerClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref PointerClick pointerClick, ref StartClickingEventComp startClickingEvent, ref BaseInputCapture baseInput, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (pointerClick.CanClick)
                {
                    switch (raycastOrigin.RayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            CheckHit(raycastOutputs.RaycastHitVar, out InteractionVariableContainer.IsClickingSomethingLeft, ERayOrigin.LEFT_HAND);
                            break;
                        case ERayOrigin.RIGHT_HAND:
                            CheckHit(raycastOutputs.RaycastHitVar, out InteractionVariableContainer.IsClickingSomethingRight, ERayOrigin.RIGHT_HAND);
                            break;
                        case ERayOrigin.CAMERA:
                            CheckHit(raycastOutputs.RaycastHitVar, out InteractionVariableContainer.IsClickingSomethingGaze, ERayOrigin.CAMERA);
                            break;
                    }
                }
            });
        }


        private void CheckHit(RaycastHitVariable hitVar, out bool hasClickSomething, ERayOrigin origin)
        {
            //If nothing is hit, we set the hasClickSomething value to false
            if (hitVar.IsNull)
            {
                hasClickSomething = false;
            }
            else
            {
                hasClickSomething = true;
                new ObjectWasClickedEvent(origin, hitVar.Value.collider.transform);
            }
        }
    }
}