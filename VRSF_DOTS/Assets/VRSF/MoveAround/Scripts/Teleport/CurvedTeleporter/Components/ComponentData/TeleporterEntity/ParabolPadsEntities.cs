using Unity.Entities;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolPadsEntities : IComponentData
    {
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
