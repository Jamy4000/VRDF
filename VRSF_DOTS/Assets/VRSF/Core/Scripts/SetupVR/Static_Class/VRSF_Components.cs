using UnityEngine;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Contains all the static references for the VRSF Objects
    /// </summary>
	public static class VRSF_Components
    {
        #region PUBLIC_VARIABLES
        [Header("The name of the SDK that has been loaded. Not necessary if you're using a Starting Screen.")]
        public static EDevice DeviceLoaded = EDevice.NONE;

        /// <summary>
        /// Whether the setup by VRSF for VR is ready
        /// </summary>
        public static bool SetupVRIsReady = false;

        public static GameObject FloorOffset;
        public static GameObject CameraRig;
        public static GameObject LeftController;
        public static GameObject RightController;
        public static GameObject VRCamera;
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Method to set the CameraRig position by taking account of the SDK loaded
        /// We suggest you to give a position situated on a plane, as we're adding the height of the user in Y axis
        /// when setYPos is true and we're not using OpenVR.
        /// </summary>
        /// <param name="newPos">The new Pos where the user should be in World coordinate</param>
        /// <param name="useVRCameraOffset">Whether we should use the VRCamera local pos to calculate the new pos of the cameraRig</param>
        /// <param name="setYPos">Wheter we have to change the Y position</param>
        public static void SetVRCameraPosition(Vector3 newPos, bool useVRCameraOffset = true, bool setYPos = true)
        {
            if (useVRCameraOffset) GetNewPosWithCameraOffset();
            CameraRig.transform.position = setYPos ? newPos : new Vector3(newPos.x, CameraRig.transform.position.y, newPos.z);


            void GetNewPosWithCameraOffset()
            {
                var y = newPos.y;
                var cameraDirectionVector = new Vector3(newPos.x - VRCamera.transform.position.x, 0.0f, newPos.z - VRCamera.transform.position.z);
                newPos = CameraRig.transform.position + cameraDirectionVector;
                newPos.y = y;
            }
        }

        /// <summary>
        /// Method to set the CameraRig position by taking account of the SDK loaded
        /// We suggest you to give a position situated on a plane, as we're adding the height of the user in Y axis
        /// when setYPos is true and we're not using OpenVR.
        /// </summary>
        /// <param name="newRotAxis"></param>
        /// <param name="angle"></param>
        /// <param name="useVRCameraOffset">Whether we should use the VRCamera local pos to calculate the new pos of the cameraRig</param>
        public static void RotateVRCameraAround(Vector3 newRotAxis, float angle)
        {
            // TODO if (useVRCameraOffset) GetNewPosWithCameraOffset();
            CameraRig.transform.RotateAround(VRCamera.transform.position, newRotAxis, angle);
        }


        /// <summary>
        /// Method to rotate the CameraRig using the offset with the VRCamera, as the VRCamera can't be rotated directly
        /// </summary>
        /// <param name="target">The point in world space we want to look at</param>
        public static void LookAtVR(Vector3 target)
        {
            var basePos = VRCamera.transform.position;
            var directionToPoint = target - VRCamera.transform.position;
            var angle = Vector3.SignedAngle(VRCamera.transform.forward, directionToPoint, Vector3.up);
            CameraRig.transform.Rotate(Vector3.up, angle);
            SetVRCameraPosition(basePos, true, false);
        }
        #endregion
    }
}