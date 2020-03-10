using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// When a StartTouchingEventComp or StopTouchingEventComp is added on an Entity, wait one frame, and then remove the component
    /// </summary>
    public class TouchingEventsRemover : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref StartTouchingEventComp startTouchingEvent) =>
            {
                if (!startTouchingEvent.HasWaitedOneFrameBeforeRemoval)
                    startTouchingEvent.HasWaitedOneFrameBeforeRemoval = true;
                else
                    PostUpdateCommands.RemoveComponent(entity, typeof(StartTouchingEventComp));
            });

            Entities.ForEach((Entity entity, ref StopTouchingEventComp stopTouchingEvent) =>
            {
                if (!stopTouchingEvent.HasWaitedOneFrameBeforeRemoval)
                    stopTouchingEvent.HasWaitedOneFrameBeforeRemoval = true;
                else
                    PostUpdateCommands.RemoveComponent(entity, typeof(StopTouchingEventComp));
            });
        }
    }
}