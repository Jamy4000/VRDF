using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Start Clicking events for CBRAs Entities
    /// </summary>
    [UpdateAfter(typeof(ClickingEventsRemover))]
    public class CBRAStopClickingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref StopClickingEventComp stopClickingEvent, ref CBRATag cbraEvents) =>
            {
                if (_entityManager.HasComponent(entity, VRInteractions.InputTypeGetter.GetTypeFor(stopClickingEvent.ButtonInteracting)) && CBRADelegatesHolder.StopClickingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}