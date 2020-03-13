using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Static class containing all variables necessary to handle the VR hovering system
    /// </summary>
	public static class HoveringVariablesContainer
    {
        /// <summary>
        /// The current GameObject hit by the Right Hand Raycaster
        /// </summary>
        private static GameObject _currentRightHoveredObject;

        /// <summary>
        /// The current GameObject hit by the Left Hand Raycaster
        /// </summary>
        private static GameObject _currentLeftHoveredObject;

        /// <summary>
        /// The current GameObject hit by the Camera Raycaster
        /// </summary>
        private static GameObject _currentGazeHoveredObject;


        public static void SetCurrentHoveredObjects(ERayOrigin rayOrigin, GameObject currentHit)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    _currentRightHoveredObject = currentHit;
                    break;
                case ERayOrigin.LEFT_HAND:
                    _currentLeftHoveredObject = currentHit;
                    break;
                case ERayOrigin.CAMERA:
                    _currentGazeHoveredObject = currentHit;
                    break;
                default:
                    throw new System.Exception();
            }
        }

        public static GameObject GetCurrentHit(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _currentRightHoveredObject;
                case ERayOrigin.LEFT_HAND:
                    return _currentLeftHoveredObject;
                case ERayOrigin.CAMERA:
                    return _currentGazeHoveredObject;
                default:
                    throw new System.Exception();
            }
        }
    }
}