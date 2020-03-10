using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Handle the Start Clicking event in VR. Basically link the Raycast, Input and Interaction System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the OnStartClickingOnObject
    /// </summary>
    public class PointerStartClickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<StartClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin raycastOrigin) =>
            {
                if (vrClicker.CanClick && !vrClicker.HasCheckedStartClickingEvent) 
                {
                    IsHittingSomethingWithClick(raycastOutputs.RaycastHitVar, raycastOrigin.RayOrigin);
                    vrClicker.HasCheckedStartClickingEvent = true;
                    vrClicker.IsClicking = true;
                }
            });

            // reset HasCheckedStartClickingEvent once the StartClickingEventComp has been removed from other systems
            Entities.WithNone<StartClickingEventComp>().ForEach((Entity entity, ref VRClicker vrClicker) =>
            {
                if (vrClicker.HasCheckedStartClickingEvent)
                    vrClicker.HasCheckedStartClickingEvent = false;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitVar"></param>
        /// <param name="hasClickSomething"></param>
        /// <param name="origin"></param>
        private void IsHittingSomethingWithClick(RaycastHitVariable hitVar, ERayOrigin origin)
        {
            //If we are currently hitting something with the raycast
            if (!hitVar.IsNull)
                new OnVRClickerStartClicking(origin, hitVar.Value.collider.gameObject);
            else
                new OnVRClickerStartClicking(origin, null);
        }
    }
}