using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the Teleport Systems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(TeleportNavMeshAuthoring))]
    public class GeneralTeleportAuthoring : MonoBehaviour
    {
        [Header("Is this teleport feature using fade out/in")]
        public bool IsUsingFadingEffect = true;

        [Header("Layer on which we shouldn't check the teleport hit")]
        public LayerMask ExcludedLayers = new LayerMask();

        /// <summary>
        /// Whether the user can use the teleport feature
        /// </summary>
        [HideInInspector] public static bool CanTeleport = true;

        /// <summary>
        /// The point where the user needs to be teleported to
        /// </summary>
        [HideInInspector] public static Vector3 PointToGoTo = Vector3.zero;
    }
} 