using Unity.Mathematics;
using UnityEngine;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Static class containing all variables necessary to handle the interactions in VR.
    /// TODO Refactor, static classes shouldn't be used in Jobs 
    /// </summary>
	public static class InteractionVariableContainer
    {
        /// <summary>
        /// Bool to verify if something is being hovered by the Right Hand Raycaster
        /// </summary>
        public static bool IsOverSomethingRight;

        /// <summary>
        /// Bool to verify if something is being hovered by the Left Hand Raycaster
        /// </summary>
        public static bool IsOverSomethingLeft;

        /// <summary>
        /// Bool to verify if something is being hovered by the Camera Raycaster
        /// </summary>
        public static bool IsOverSomethingGaze;


        /// <summary>
        /// The current GameObject hit by the Right Hand Raycaster
        /// </summary>
        public static GameObject CurrentRightHit;

        /// <summary>
        /// The current GameObject hit by the Left Hand Raycaster
        /// </summary>
        public static GameObject CurrentLeftHit;

        /// <summary>
        /// The current GameObject hit by the Camera Raycaster
        /// </summary>
        public static GameObject CurrentGazeHit;


        /// <summary>
        /// The current hit position of the Right Hand Raycaster
        /// </summary>
        public static float3 CurrentRightHitPosition;

        /// <summary>
        /// The current hit position of the Left Hand Raycaster
        /// </summary>
        public static float3 CurrentLeftHitPosition;

        /// <summary>
        /// The current hit position of the Camera Raycaster
        /// </summary>
        public static float3 CurrentGazeHitPosition;
    }
}