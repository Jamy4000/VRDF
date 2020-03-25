using Unity.Entities;
using VRDF.Core.Inputs;
using VRDF.Core.Raycast;

namespace VRDF.Core.VRClicker
{
    /// <summary>
    /// Handle the Start Clicking event in VR. Basically link the Raycast, Input and Interaction System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the OnStartClickingOnObject
    /// </summary>
    [UpdateAfter(typeof(ClickingEventsRemover))]
    public class VRClickerStartClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<StartClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (vrClicker.CanClick)
                {
                    var hitVar = raycastOutputs.RaycastHitVar;
                    new OnVRClickerStartClicking(raycastOrigin.RayOrigin, hitVar.IsNull ? null : hitVar.Value.collider.gameObject);
                    vrClicker.IsClicking = true;
                }
            });
        }
    }
}