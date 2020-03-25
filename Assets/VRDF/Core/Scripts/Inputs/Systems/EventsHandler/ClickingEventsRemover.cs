using Unity.Entities;

namespace VRDF.Core.Inputs
{
    /// <summary>
    /// When a StartClickingEventComp or StopClickingEventComp is added on an Entity, wait one frame, and then remove the component
    /// </summary>
    public class ClickingEventsRemover : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref StartClickingEventComp startClickingEvent) =>
            {
                if (!startClickingEvent.HasWaitedOneFrameBeforeRemoval)
                    startClickingEvent.HasWaitedOneFrameBeforeRemoval = true;
                else
                    PostUpdateCommands.RemoveComponent(entity, typeof(StartClickingEventComp));
            });

            Entities.ForEach((Entity entity, ref StopClickingEventComp stopClickingEvent) =>
            {
                if (!stopClickingEvent.HasWaitedOneFrameBeforeRemoval)
                    stopClickingEvent.HasWaitedOneFrameBeforeRemoval = true;
                else
                    PostUpdateCommands.RemoveComponent(entity, typeof(StopClickingEventComp));
            });
        }
    }
}