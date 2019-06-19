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
        public static EDevice DeviceLoaded = EDevice.NULL;

        /// <summary>
        /// Whether the setup by VRSF for VR is ready
        /// </summary>
        public static bool SetupVRIsReady = false;

        public static Transform FloorOffset;
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
        public static void SetCameraRigPosition(Vector3 newPos, bool useVRCameraOffset = true, bool setYPos = true)
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
        #endregion
    }
}