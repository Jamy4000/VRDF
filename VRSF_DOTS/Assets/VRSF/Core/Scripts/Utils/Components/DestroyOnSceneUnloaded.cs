using Unity.Entities;

namespace VRSF.Core
{
    /// <summary>
    /// Tag to let us know that an entity is part of the VRSF Framework, and can be destroy when reloading a scene.
    /// </summary>
    public struct DestroyOnSceneUnloaded : IComponentData 
    {
        /// <summary>
        /// The ActiveScene where this entity was created
        /// </summary>
        public int SceneIndex;
    }
}