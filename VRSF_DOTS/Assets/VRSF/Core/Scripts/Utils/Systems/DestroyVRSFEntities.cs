using Unity.Entities;
using UnityEngine.SceneManagement;

namespace VRSF.Core.Utils
{
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
            Entities.ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent) =>
            {
                EntityManager.DestroyEntity(e);
            });
        }
    }
}