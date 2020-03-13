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
                    new OnVRClickerStopClicking(raycastOrigin.RayOrigin, VRClickerVariablesContainer.GetCurrentClickedObject(raycastOrigin.RayOrigin));
                    vrClicker.IsClicking = false;
                }
            });
        }
    }
}