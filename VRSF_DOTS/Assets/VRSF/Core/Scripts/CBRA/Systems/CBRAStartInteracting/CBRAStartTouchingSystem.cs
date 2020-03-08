using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Start Touching events for CBRAs Entities
    /// </summary>
    public class CBRAStartTouchingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<CBRATag>().ForEach((Entity entity, ref StartTouchingEventComp startTouchingEvent) =>
            {
                if (_entityManager.HasComponent(entity, VRInteractions.InputTypeGetter.GetTypeFor(startTouchingEvent.ButtonInteracting)) && CBRADelegatesHolder.StartTouchingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });

            // As StartTouchingEventComp is only used by this system to raise the event one time, we remove it as soon as we're done raising the event.
            Entities.ForEach((Entity entity, ref StartTouchingEventComp startTouchingEvent) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StartTouchingEventComp));
            });
        }
    }
}