using Unity.Mathematics;
using UnityEngine;
using VRDF.Core.Raycast;

namespace VRDF.Core.VRInteractions
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
        private static bool _isOverSomethingRight;

        /// <summary>
        /// Bool to verify if something is being hovered by the Left Hand Raycaster
        /// </summary>
        private static bool _isOverSomethingLeft;

        /// <summary>
        /// Bool to verify if something is being hovered by the Camera Raycaster
        /// </summary>
        private static bool _isOverSomethingGaze;


        /// <summary>
        /// The current GameObject hit by the Right Hand Raycaster
        /// </summary>
        private static GameObject _currentRightHit;

        /// <summary>
        /// The current GameObject hit by the Left Hand Raycaster
        /// </summary>
        private static GameObject _currentLeftHit;

        /// <summary>
        /// The current GameObject hit by the Camera Raycaster
        /// </summary>
        private static GameObject _currentGazeHit;


        /// <summary>
        /// The current hit position of the Right Hand Raycaster
        /// </summary>
        private static float3 _currentRightHitPosition;

        /// <summary>
        /// The current hit position of the Left Hand Raycaster
        /// </summary>
        private static float3 _currentLeftHitPosition;

        /// <summary>
        /// The current hit position of the Camera Raycaster
        /// </summary>
        private static float3 _currentGazeHitPosition;


        public static void SetInteractionVariables(ERayOrigin rayOrigin, Vector3 hitPoint, GameObject currentHit)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    _currentRightHitPosition = hitPoint;
                    _currentRightHit = currentHit;
                    _isOverSomethingRight = currentHit != null;
                    break;
                case ERayOrigin.LEFT_HAND:
                    _currentLeftHitPosition = hitPoint;
                    _currentLeftHit = currentHit;
                    _isOverSomethingLeft = currentHit != null;
                    break;
                case ERayOrigin.CAMERA:
                    _currentGazeHitPosition = hitPoint;
                    _currentGazeHit = currentHit;
                    _isOverSomethingGaze = currentHit != null;
                    break;
                default:
                    throw new System.Exception();
            }
        }

        public static float3 GetCurrentHitPosition(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _currentRightHitPosition;
                case ERayOrigin.LEFT_HAND:
                    return _currentLeftHitPosition;
                case ERayOrigin.CAMERA:
                    return _currentGazeHitPosition;
                default:
                    throw new System.Exception();
            }
        }

        public static GameObject GetCurrentHit(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _currentRightHit;
                case ERayOrigin.LEFT_HAND:
                    return _currentLeftHit;
                case ERayOrigin.CAMERA:
                    return _currentGazeHit;
                default:
                    throw new System.Exception();
            }
        }

        public static bool GetIsOverSomething(ERayOrigin rayOrigin)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    return _isOverSomethingRight;
                case ERayOrigin.LEFT_HAND:
                    return _isOverSomethingLeft;
                case ERayOrigin.CAMERA:
                    return _isOverSomethingGaze;
                default:
                    throw new System.Exception();
            }
        }
    }
}