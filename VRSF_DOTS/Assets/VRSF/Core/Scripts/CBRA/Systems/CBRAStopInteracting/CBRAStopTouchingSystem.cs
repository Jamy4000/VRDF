using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the Stop Touching events for CBRAs Entities
    /// </summary>
    public class CBRAStopTouchingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref StopTouchingEventComp stopTouchingEvent, ref CBRAEventComponent cbraEvents) =>
            {
                if (!cbraEvents.HasCheckedStopTouchingEvent && _entityManager.HasComponent(entity, VRInteractions.InputTypeGetter.GetTypeFor(stopTouchingEvent.ButtonInteracting)) && CBRADelegatesHolder.StopTouchingEvents.TryGetValue(entity, out System.Action action))
                {
                    cbraEvents.HasCheckedStopTouchingEvent = true;
                    action.Invoke();
                }
            });

            Entities.WithNone<StopTouchingEventComp>().ForEach((Entity entity, ref CBRAEventComponent cbraEvents) =>
            {
                if (cbraEvents.HasCheckedStopTouchingEvent)
                    cbraEvents.HasCheckedStopTouchingEvent = false;
            });
        }
    }
}