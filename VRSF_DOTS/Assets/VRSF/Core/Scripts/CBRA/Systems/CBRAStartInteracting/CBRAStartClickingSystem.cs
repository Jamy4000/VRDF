using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CBRAStartClickingSystem : ComponentSystem
    {
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            // Cache the EntityManager in a field, so we don't have to get it every frame
            _entityManager = World.Active.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType, ref StartClickingEventComp startClickingEvent) =>
            {
                if (CBRADelegatesHolder.StartClickingEvents.ContainsKey(entity) && _entityManager.HasComponent(entity, CBRAInputTypeGetter.GetTypeFor(startClickingEvent.ButtonInteracting)))
                    CBRADelegatesHolder.StartClickingEvents[entity].Invoke();
            });

            Entities.ForEach((Entity entity, ref StartClickingEventComp startClickingEvent) =>
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(StartClickingEventComp));
            });
        }
    }
}