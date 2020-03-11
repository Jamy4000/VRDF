using UnityEngine;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Static class containing all variables to keep track of the currently clicked object
    /// </summary>
	public static class VRClickerVariablesContainer
    {
        /// <summary>
        /// Bool to verify if something is being clicked with a Right Hand Raycaster
        /// </summary>
        public static bool IsClickingRight;

        /// <summary>
        /// Bool to verify if something is being clicked with a Left Hand Raycaster
        /// </summary>
        public static bool IsClickingLeft;

        /// <summary>
        /// Bool to verify if something is being clicked with a Camera Raycaster
        /// </summary>
        public static bool IsClickingGaze;


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