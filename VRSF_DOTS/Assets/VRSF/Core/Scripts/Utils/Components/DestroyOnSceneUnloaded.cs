using Unity.Entities;

namespace VRSF.Core.Utils
{
    /// <summary>
    /// Tag to let us know that an entity is part of the VRSF Framework, and can be destroy when reloading a scene.
    /// </summary>
    public struct DestroyOnSceneUnloaded : IComponentData 
    {
        public int SceneIndex;
    }
}