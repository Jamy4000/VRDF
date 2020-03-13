using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Handle the Click event in VR. Basically link the Raycast system and the Input System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the ObjectWasClickedEvent
    /// </summary>
    public class VRClickerClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<StartClickingEventComp, StopClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (vrClicker.CanClick && vrClicker.IsClicking)
                    new OnVRClickerIsClicking(raycastOrigin.RayOrigin, VRClickerVariablesContainer.GetCurrentClickedObject(raycastOrigin.RayOrigin));
            });
        }
    }
}