using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Handle the Start Clicking event in VR. Basically link the Raycast, Input and Interaction System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the OnStartClickingOnObject
    /// </summary>
    [UpdateAfter(typeof(ClickingEventsRemover))]
    public class VRClickerStopClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<StopClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (vrClicker.CanClick)
                {
                    switch (raycastOrigin.RayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            new OnVRClickerStopClicking(ERayOrigin.LEFT_HAND, VRClickerVariablesContainer.CurrentClickedObjectLeft);
                            break;
                        case ERayOrigin.RIGHT_HAND:
                            new OnVRClickerStopClicking(ERayOrigin.RIGHT_HAND, VRClickerVariablesContainer.CurrentClickedObjectRight);
                            break;
                        case ERayOrigin.CAMERA:
                            new OnVRClickerStopClicking(ERayOrigin.CAMERA, VRClickerVariablesContainer.CurrentClickedObjectGaze);
                            break;
                    }

                    vrClicker.IsClicking = false;
                }
            });
        }
    }
}