using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CBRAStartTouchingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType, ref StartTouchingEventComp startTouchingEvent) =>
            {
                if (CBRADelegatesHolder.StartTouchingEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, CBRAInputTypeGetter.GetTypeFor(startTouchingEvent.ButtonInteracting)))
                    CBRADelegatesHolder.StartTouchingEvents[entity].Invoke();
            });

            Entities.ForEach((Entity entity, ref StartTouchingEventComp startTouchingEvent) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StartTouchingEventComp));
            });
        }
    }
}