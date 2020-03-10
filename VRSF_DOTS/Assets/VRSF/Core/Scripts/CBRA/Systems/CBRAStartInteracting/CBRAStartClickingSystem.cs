using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Start Clicking events for CBRAs Entities
    /// </summary>
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
            Entities.ForEach((Entity entity, ref StartClickingEventComp startClickingEvent, ref CBRAEventComponent cbraEvent) =>
            {
                if (!cbraEvent.HasCheckedStartClickingEvent && _entityManager.HasComponent(entity, VRInteractions.InputTypeGetter.GetTypeFor(startClickingEvent.ButtonInteracting)) && CBRADelegatesHolder.StartClickingEvents.TryGetValue(entity, out System.Action action))
                {
                    cbraEvent.HasCheckedStartClickingEvent = true;
                    action.Invoke();
                }
            });

            Entities.WithNone<StartClickingEventComp>().ForEach((Entity entity, ref CBRAEventComponent cbraEvents) =>
            {
                if (cbraEvents.HasCheckedStartClickingEvent)
                    cbraEvents.HasCheckedStartClickingEvent = false;
            });
        }
    }
}