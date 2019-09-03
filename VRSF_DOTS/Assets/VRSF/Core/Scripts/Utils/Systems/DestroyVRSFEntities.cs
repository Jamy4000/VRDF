using Unity.Entities;
using UnityEngine.SceneManagement;

namespace VRSF.Core.Utils
{
    public class EntitiesDestroyOnSceneLoaded : ComponentSystem
    {
        private string _activeScene;

        protected override void OnCreate()
        {
            base.OnCreate();
            SceneManager.sceneUnloaded += DestroyEntities;
            SceneManager.sceneLoaded += SetActiveScene;
            _activeScene = SceneManager.GetActiveScene().name;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= DestroyEntities;
            SceneManager.sceneLoaded -= SetActiveScene;
        }

        private void DestroyEntities(Scene unloadedScene)
        {
            if (unloadedScene.name != _activeScene)
                return;

            Entities.ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent) =>
            {
                EntityManager.DestroyEntity(e);
            });

            Entities.ForEach((Entity e, ref DestroyOnSceneUnloaded destroyComponent, ref Disabled disabledEntityComp) =>
            {
                EntityManager.DestroyEntity(e);
            });
        }

        private void SetActiveScene(Scene loadedScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
                _activeScene = loadedScene.name;
        }
    }
}