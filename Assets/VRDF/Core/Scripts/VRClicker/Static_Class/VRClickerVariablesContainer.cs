using UnityEngine;
using VRDF.Core.Raycast;

namespace VRDF.Core.VRClicker
{
    /// <summary>
    /// Static class containing all variables to keep track of the currently clicked object
    /// </summary>
	public static class VRClickerVariablesContainer
    {
        /// <summary>
        /// Bool to verify if something is being clicked with a Right Hand Raycaster
        /// </summary>
        private static bool _isClickingRight;

        /// <summary>
        /// Bool to verify if something is being clicked with a Left Hand Raycaster
        /// </summary>
        private static bool _isClickingLeft;

        /// <summary>
        /// Bool to verify if something is being clicked with a Camera Raycaster
        /// </summary>
        private static bool _isClickingGaze;


        /// <summary>
        /// The currentclicked GameObject using the Right Hand Raycaster with a VR Clicker
        /// </summary>
        private static GameObject _currentClickedObjectRight;

        /// <summary>
        /// The current clicked GameObject using the Left Hand Raycaster with a VR Clicker
        /// </summary>
        private static GameObject _currentClickedObjectLeft;

        /// <summary>
        /// The currently clicked GameObject using the Camera Raycaster with a VR Clicker
        /// </summary>
        private static GameObject _currentClickedObjectGaze;

        public static GameObject GetCurrentClickedObject(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _currentClickedObjectRight;
                case ERayOrigin.LEFT_HAND:
                    return _currentClickedObjectLeft;
                case ERayOrigin.CAMERA:
                    return _currentClickedObjectGaze;
                default:
                    throw new System.Exception();
            }
        }

        public static bool IsClicking(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _isClickingRight;
                case ERayOrigin.LEFT_HAND:
                    return _isClickingLeft;
                case ERayOrigin.CAMERA:
                    return _isClickingGaze;
                default:
                    throw new System.Exception();
            }
        }

        public static void SetCurrentClickedVariables(ERayOrigin rayOrigin, GameObject currentClickedObject, bool isClicking)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    _currentClickedObjectRight = currentClickedObject;
                    _isClickingRight = isClicking;
                    break;
                case ERayOrigin.LEFT_HAND:
                    _currentClickedObjectLeft = currentClickedObject;
                    _isClickingLeft = isClicking;
                    break;
                case ERayOrigin.CAMERA:
                    _currentClickedObjectGaze = currentClickedObject;
                    _isClickingGaze = isClicking;
                    break;
                default:
                    throw new System.Exception();
            }
        }
    }
}