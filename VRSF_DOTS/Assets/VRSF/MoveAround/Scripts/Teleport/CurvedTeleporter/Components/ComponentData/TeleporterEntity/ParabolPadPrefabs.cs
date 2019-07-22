using Unity.Entities;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolObjects : IComponentData
    {
        /// <summary>
        /// Prefab to use as the selection pad when the player is pointing at a valid teleportable surface.
        /// </summary>
        public Entity SelectionPadPrefab;

        /// <summary>
        /// Prefab to use as the selection pad when the player is pointing at an invalid teleportable surface.
        /// </summary>
        public Entity InvalidPadPrefab;

        /// <summary>
        /// Instance Reference to the selection pad when the player is pointing at a valid teleportable surface.
        /// </summary>
        public Entity SelectionPadInstance;

        /// <summary>
        /// Instance Reference to the selection pad when the player is pointing at an invalid teleportable surface.
        /// </summary>
        public Entity InvalidPadInstance;
    }
}
