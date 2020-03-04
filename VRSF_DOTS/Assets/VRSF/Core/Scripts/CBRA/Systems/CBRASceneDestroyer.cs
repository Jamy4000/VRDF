using Unity.Entities;
using UnityEngine.SceneManagement;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Same script as DestroyVRSFEntities, but only for CBRA entities.
    /// It helps us remove the referece to the entity in the CBRADelegateHolder.
    /// </summary>
    public class CBRASceneDestroyer : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            SceneManager.sceneUnloaded += DestroyEntities;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= DestroyEntities;
        }

        private void DestroyEntities(Scene unloadedScene)
        {
            Entities.WithAll<CBRATag>().ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent) =>
            {
                if (unloadedScene.buildIndex == destroyComponent.SceneIndex)
                {
                    RemoveEvents(e);
                    EntityManager.DestroyEntity(e);
                }
            });

            Entities.WithAll<CBRATag>().ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent, ref Disabled disabledEntityComp) =>
            {
                if (unloadedScene.buildIndex == destroyComponent.SceneIndex)
                {
                    RemoveEvents(e);
                    EntityManager.DestroyEntity(e);
                }
            });
        }

        private void RemoveEvents(Entity e)
        {
            CBRADelegatesHolder.StartClickingEvents.Remove(e);
            CBRADelegatesHolder.IsClickingEvents.Remove(e);
            CBRADelegatesHolder.StopClickingEvents.Remove(e);
            CBRADelegatesHolder.StartTouchingEvents.Remove(e);
            CBRADelegatesHolder.IsTouchingEvents.Remove(e);
            CBRADelegatesHolder.StopTouchingEvents.Remove(e);
        }
    }
}