using Unity.Entities;
using UnityEngine.SceneManagement;

namespace VRSF.Core
{
    /// <summary>
    /// Destroy entities with the component DestroyOnSceneUnloaded when the Active Scene change.
    /// </summary>
    public class EntitiesDestroyOnSceneLoaded : ComponentSystem
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
            Entities.WithNone<CBRA.CBRATag>().ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent) =>
            {
                if (unloadedScene.buildIndex == destroyComponent.SceneIndex)
                    EntityManager.DestroyEntity(e);
            });

            Entities.WithNone<CBRA.CBRATag>().ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent, ref Disabled disabledEntityComp) =>
            {
                if (unloadedScene.buildIndex == destroyComponent.SceneIndex)
                    EntityManager.DestroyEntity(e);
            });
        }
    }
}