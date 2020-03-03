using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using VRSF.Core.SetupVR;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeTransformationSystem : ComponentSystem
    {
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

            var pos = _camera.transform.position + _camera.transform.forward * 0.3f;
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

        private void SetCameraRef()
        {
            if (VRSF_Components.VRCamera != null)
            {
                _camera = VRSF_Components.VRCamera.transform;
            }
            else
            {
                var camObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (camObject != null)
                    _camera = camObject.transform;
                else
                    Debug.Log("<b>[VRSF] :</b> Couldn't find any Camera with tag MainCamera.");
            }
        }
    }
}