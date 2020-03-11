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
                    new OnVRClickerIsClicking(raycastOrigin.RayOrigin, GetCurrentlyClickedObject(raycastOrigin.RayOrigin));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitVar"></param>
        /// <param name="hasClickSomething"></param>
        /// <param name="origin"></param>
        /// <returns>True if the user is still clicking on the same object</returns>
        private UnityEngine.GameObject GetCurrentlyClickedObject(ERayOrigin origin)
        {
            switch (origin)
            {
                case ERayOrigin.LEFT_HAND:
                    return VRClickerVariablesContainer.CurrentClickedObjectLeft;
                case ERayOrigin.RIGHT_HAND:
                    return VRClickerVariablesContainer.CurrentClickedObjectRight;
                case ERayOrigin.CAMERA:
                    return VRClickerVariablesContainer.CurrentClickedObjectGaze;
                default:
                    throw new System.Exception();
            }
        }
    }
}