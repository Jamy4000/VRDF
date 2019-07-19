using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public struct GeneralTeleportParameters : IComponentData
    {
        /// <summary>
        /// Is this teleport feature using fade out/in
        /// </summary>
        public bool IsUsingFadingEffect;

        /// <summary>
        /// Indicates the current use of teleportation.
        /// None: The player is not using teleportation right now
        /// Selecting: The player is currently selecting a teleport destination (holding down on touchpad)
        /// Teleporting: The player has selected a teleport destination and is currently teleporting now (fading in/out)
        /// </summary>
        public ETeleportState CurrentTeleportState;

        public bool HasTeleported;
    }
}