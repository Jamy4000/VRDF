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
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType, ref StopTouchingEventComp stopTouchingEvent) =>
            {
                if (_entityManager.HasComponent(entity, CBRAInputTypeGetter.GetTypeFor(stopTouchingEvent.ButtonInteracting)))
                    CBRADelegatesHolder.StartTouchingEvents[entity]?.Invoke();

                PostUpdateCommands.RemoveComponent(entity, typeof(StopTouchingEventComp));
            });
        }
    }
}