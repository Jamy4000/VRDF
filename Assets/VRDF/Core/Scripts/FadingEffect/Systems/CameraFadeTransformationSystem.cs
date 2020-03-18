using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace VRDF.Core.FadingEffect
{
    /// <summary>
    /// Handle the position of the Fading Plane, so it's always in front the user's eyes
    /// </summary>
    public class CameraFadeTransformationSystem : ComponentSystem
    {
        /// <summary>
        /// Reference to the Main Camera in the scene
        /// </summary>
        private Transform _camera;

        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += InitSetupVR;
            StartFadingInEvent.Listeners += InitFadingIn;
            StartFadingOutEvent.Listeners += InitFadingOut;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (_camera == null)
                return;

            // we place the plane 0.3 meter in front of the camera
            var pos = _camera.transform.position + _camera.transform.forward * 0.3f;
            // We always face the camera
            var rot = Quaternion.LookRotation(_camera.transform.up, _camera.transform.forward);

            Entities.ForEach((ref Translation translation, ref Rotation rotation, ref CameraFadeParameters _) =>
            {
                // Place the instantiated canvas in front of the camera
                translation.Value = pos;

                // Rotate the instantiated canvas to face the camera
                rotation.Value = rot;
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _camera = null;
            OnSetupVRReady.Listeners -= InitSetupVR;
            StartFadingInEvent.Listeners -= InitFadingIn;
            StartFadingOutEvent.Listeners -= InitFadingOut;
        }

        private void InitSetupVR(OnSetupVRReady _)
        {
            SetCameraRef();
        }

        private void InitFadingIn(StartFadingInEvent _)
        {
            if (_camera == null)
                SetCameraRef();
        }

        private void InitFadingOut(StartFadingOutEvent _)
        {
            if (_camera == null)
                SetCameraRef();
        }

        /// <summary>
        /// Check if the camera is correctly set, and f not, assign the new camera's transform
        /// </summary>
        private void SetCameraRef()
        {
            if (VRDF_Components.VRCamera != null)
            {
                _camera = VRDF_Components.VRCamera.transform;
            }
            else
            {
                // In case we're not in VR, because why not
                var camObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (camObject != null)
                    _camera = camObject.transform;
                else
                    Debug.LogError("<Color=red><b>[VRDF] :</b> Couldn't find any Camera with tag MainCamera.</Color>");
            }
        }
    }
}