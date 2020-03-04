using UnityEngine;
using Unity.Entities;

namespace VRSF.Core
{
    public static class OnSceneUnloadedEntityDestroyer
    {
        /// <summary>
        /// Check if the gameObject using a DestroyOnSceneUnload component isn't place on a gameObject that will be placed on a DontDestroyOnLoad scene
        /// </summary>
        public static void CheckDestroyOnSceneUnload(EntityManager entityManager, Entity entity, int gameObjectSceneIndex, string componentName)
        {
            // Need to check for index in case the object is placed in a DontDestroyOnLoad scene
            if (gameObjectSceneIndex != -1)
            {
                Debug.LogFormat("<Color=yellow><b>[VRSF] :</b> Placing a {0} on an gameObject that is then set to DontDestroyOnLoad is not adviced, as it may lead to unexpected behaviour.\n" +
                    "Please consider unchecking the 'Destroy On Scene Unloaded' checkbox, put this {0} component in the main Scene, or only call the DontDestroyOnLoad method once this component is setup.</Color>", componentName);
                entityManager.SetComponentData(entity, new DestroyOnSceneUnloaded { SceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex });
            }
            else
            {
                entityManager.SetComponentData(entity, new DestroyOnSceneUnloaded { SceneIndex = gameObjectSceneIndex });
            }
        }
    }
}