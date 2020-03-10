using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Handle the Start Clicking event in VR. Basically link the Raycast, Input and Interaction System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the OnStartClickingOnObject
    /// </summary>
    public class PointerStopClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<StopClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (vrClicker.CanClick && !vrClicker.HasCheckedStopClickingEvent)
                {
                    switch (raycastOrigin.RayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            new OnVRClickerStopClicking(ERayOrigin.LEFT_HAND, VRInteractions.InteractionVariableContainer.CurrentClickedObjectLeft);
                            break;
                        case ERayOrigin.RIGHT_HAND:
                            new OnVRClickerStopClicking(ERayOrigin.RIGHT_HAND, VRInteractions.InteractionVariableContainer.CurrentClickedObjectRight);
                            break;
                        case ERayOrigin.CAMERA:
                            new OnVRClickerStopClicking(ERayOrigin.CAMERA, VRInteractions.InteractionVariableContainer.CurrentClickedObjectGaze);
                            break;
                    }

                    vrClicker.HasCheckedStopClickingEvent = true;
                    vrClicker.IsClicking = false;
                }
            });

            // reset HasCheckedStopClickingEvent once the StartClickingEventComp has been removed from other systems
            Entities.WithNone<StopClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker) =>
            {
                if (vrClicker.HasCheckedStopClickingEvent)
                    vrClicker.HasCheckedStopClickingEvent = false;
            });
        }
    }
}