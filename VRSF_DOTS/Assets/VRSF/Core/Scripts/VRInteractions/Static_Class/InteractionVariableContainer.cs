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
        /// Bool to verify if something is being clicked with a Right Hand Raycaster
        /// </summary>
        public static bool IsClickingSomethingRight;

        /// <summary>
        /// Bool to verify if something is being clicked with a Left Hand Raycaster
        /// </summary>
        public static bool IsClickingSomethingLeft;

        /// <summary>
        /// Bool to verify if something is being clicked with a Camera Raycaster
        /// </summary>
        public static bool IsClickingSomethingGaze;


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


        /// <summary>
        /// The currentclicked GameObject using the Right Hand Raycaster with a VR Clicker
        /// </summary>
        public static GameObject CurrentClickedObjectRight;

        /// <summary>
        /// The current clicked GameObject using the Left Hand Raycaster with a VR Clicker
        /// </summary>
        public static GameObject CurrentClickedObjectLeft;

        /// <summary>
        /// The currently clicked GameObject using the Camera Raycaster with a VR Clicker
        /// </summary>
        public static GameObject CurrentClickedObjectGaze;
    }
}