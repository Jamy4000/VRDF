using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;

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
            Entities.ForEach((ref PointerClick pointerClick, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin, ref BaseInputCapture bic) =>
            {
                if (pointerClick.CanClick && bic.IsClicking)
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

            // As StartClickingEventComp is only used by this system to raise the event one time, we remove it as soon as we're done raising the event.
            Entities.WithAll<StartClickingEventComp, PointerClick>().ForEach((Entity entity, ref StartClickingEventComp startClickingEvent) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StartClickingEventComp));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitVar"></param>
        /// <param name="hasClickSomething"></param>
        /// <param name="origin"></param>
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
                new ObjectIsBeingClickedEvent(origin, hitVar.Value.collider.gameObject);
            }
        }
    }
}