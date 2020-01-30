using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;
using E7.DataStructure;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Simulator
{
    [UpdateAfter(typeof(SimulatorRotationInterpolationSystem))]
    public class SimulatorMovementSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
        }
        
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref SimulatorMovementSpeed sms, ref SimulatorMovementRotation smr, ref SimulatorCameraState scs, ref JobAnimationCurve jac) =>
            {
                // Translation
                if (EvaluateTranslation(sms, ref scs, deltaTime))
                {
                    // Interpolate toward new position
                    Interpolate(sms, smr, ref scs, deltaTime);
                }
            });
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        /// <summary>
        /// Evaluate the camera translation based on WASD and apply boost.
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="deltaTime"></param>
        /// <returns>return true if translation is not a vector3.zero</returns>
        private bool EvaluateTranslation(SimulatorMovementSpeed sms, ref SimulatorCameraState scs, float deltaTime)
        {
            var translation = GetInputTranslationDirection() * sms.BaseSpeed * deltaTime;
            if (translation != Vector3.zero)
            {
                SimulatorCameraStateSystem.ResetCameraStateTransform(ref scs);

                // Speed up movement when shift key held
                if (Input.GetKey(KeyCode.LeftShift))
                    // Modify movement by a boost factor (defined in Inspector and modified in play mode through up and down arrow)
                    translation *= Mathf.Pow(2.0f, sms.LeftShiftBoost);

                scs.TargetCameraState.Translate(translation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get inputs to check if the user wanna move in the scene
        /// </summary>
        /// <returns>The direction vector based on the user's inputs</returns>
        private Vector3 GetInputTranslationDirection()
        {
            // If the mouse is hovering a UI Element
            if (InteractionVariableContainer.CurrentGazeHit != null && InteractionVariableContainer.CurrentGazeHit.GetComponent<UnityEngine.UI.Selectable>() != null)
                return Vector3.zero;

            Vector3 direction = Vector3.zero;
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // Forward or backward
            if (vertical != 0.0f)
                direction.z += vertical;

            // left or right
            if (horizontal != 0.0f)
                direction.x += horizontal;

            // up or down
            if (Input.GetKey(KeyCode.Q))
                direction += Vector3.down;
            if (Input.GetKey(KeyCode.E))
                direction += Vector3.up;

            return direction;
        }

        /// <summary>
        /// Framerate-independent interpolation for position and Rotation
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="scs"></param>
        /// <param name="smr"></param>
        /// <param name="deltaTime"></param>
        public static void Interpolate(SimulatorMovementSpeed sms, SimulatorMovementRotation smr, ref SimulatorCameraState scs, float deltaTime)
        {
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / sms.PositionLerpTime * deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / smr.RotationLerpTime * deltaTime);
            scs.InterpolatingCameraState.LerpTowards(scs.TargetCameraState, positionLerpPct, rotationLerpPct);
            scs.InterpolatingCameraState.UpdateTransform(VRSF_Components.CameraRig.transform);
        }

        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}