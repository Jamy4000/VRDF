using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CBRAStopTouchingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CBRATag cbraTag, ref StopTouchingEventComp stopTouchingEvent) =>
            {
                if (_entityManager.HasComponent(entity, CBRAInputTypeGetter.GetTypeFor(stopTouchingEvent.ButtonInteracting)))
                    CBRADelegatesHolder.StopTouchingEvents[entity]?.Invoke();
            });

            Entities.ForEach((Entity entity, ref StopTouchingEventComp stopTouchingEvent) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StopTouchingEventComp));
            });
        }
    }
}