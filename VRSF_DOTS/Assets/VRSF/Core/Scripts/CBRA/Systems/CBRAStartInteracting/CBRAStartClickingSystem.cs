using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Start Clicking events for CBRAs Entities
    /// </summary>
    [UpdateAfter(typeof(VRClicker.PointerClickingSystem))]
    public class CBRAStartClickingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<CBRATag>().ForEach((Entity entity, ref StartClickingEventComp startClickingEvent) =>
            {
                if (_entityManager.HasComponent(entity, VRInteractions.InputTypeGetter.GetTypeFor(startClickingEvent.ButtonInteracting)) && CBRADelegatesHolder.StartClickingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });

            // As StartClickingEventComp is only used by this system to raise the event one time, we remove it as soon as we're done raising the event.
            Entities.WithAll<CBRATag, StartClickingEventComp>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StartClickingEventComp));
            });
        }
    }
}