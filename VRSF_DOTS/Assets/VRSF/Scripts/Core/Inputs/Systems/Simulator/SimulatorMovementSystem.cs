using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class SimulatorMovementSystem : ComponentSystem
    {
        struct Filter
        {
            public SimulatorMovementComponent cameraComponent;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckSystemState;
            base.OnCreateManager();
        }
        
        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;
            Vector2 mouseMovements = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            foreach (var e in GetEntities<Filter>())
            {
                // Check for mouse scroll wheel going up or down, and set shift boost based on that
                if (Input.GetAxis("Mouse Scrollwheel") > 0)
                    e.cameraComponent.LeftShiftBoost += 0.2f;
                if (Input.GetAxis("Mouse Scrollwheel") < 0)
                    e.cameraComponent.LeftShiftBoost -= 0.2f;

                // Check if the user is rotating the camera with the mouse wheel
                CheckForRotation(e.cameraComponent, dt, mouseMovements);

                // Translation
                if (EvaluateTranslation(e.cameraComponent, dt))
                {
                    // Interpolate toward new position
                    Interpolate(e.cameraComponent, dt);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= CheckSystemState;
        }

        private void CheckForRotation(SimulatorMovementComponent camComp, float deltaTime, Vector2 mouseMovements)
        {
            // Deactivate cursor visibility and lock it when starting to rotate
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                ResetCameraStateTransform(camComp);
            }
            // Reactivate cursor visibility and delock it when stopping to rotate
            else if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            // Rotation with right click
            else if (Input.GetMouseButton(1))
            {
                if (camComp.ReversedYAxis)
                    mouseMovements.y = -mouseMovements.y;
                
                // Evaluate the camera rotation based on the mouse screen position
                EvaluateRotation();

                // Interpolate toward new position
                Interpolate(camComp, deltaTime);
            }

            void EvaluateRotation()
            {
                var mouseSensitivityFactor = camComp.MouseSensitivityCurve.Evaluate(mouseMovements.magnitude);
                camComp.m_TargetCameraState.yaw += mouseMovements.x * mouseSensitivityFactor;
                camComp.m_TargetCameraState.pitch += mouseMovements.y * mouseSensitivityFactor;
            }
        } 
        
        /// <summary>
        /// Evaluate the camera translation based on WASD and apply boost.
        /// </summary>
        /// <param name="cameraComp"></param>
        /// <param name="deltaTime"></param>
        /// <returns>return true if translation is not a vector3.zero</returns>
        private bool EvaluateTranslation(SimulatorMovementComponent cameraComp, float deltaTime)
        {
            var translation = GetInputTranslationDirection() * cameraComp.BaseSpeed * deltaTime;
            if (translation != Vector3.zero)
            {
                ResetCameraStateTransform(cameraComp);

                // Speed up movement when shift key held
                if (Input.GetKey(KeyCode.LeftShift))
                    // Modify movement by a boost factor (defined in Inspector and modified in play mode through up and down arrow)
                    translation *= Mathf.Pow(2.0f, cameraComp.LeftShiftBoost);
                
                cameraComp.m_TargetCameraState.Translate(translation);

                return true;
            }
            return false;
        }

        // Framerate-independent interpolation for position
        private void Interpolate(SimulatorMovementComponent cameraComp, float deltaTime)
        {
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / cameraComp.PositionLerpTime) * deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / cameraComp.RotationLerpTime) * deltaTime);
            cameraComp.m_InterpolatingCameraState.LerpTowards(cameraComp.m_TargetCameraState, positionLerpPct, rotationLerpPct);
            cameraComp.m_InterpolatingCameraState.UpdateTransform(cameraComp.transform);
        }

        /// <summary>
        /// Get inputs to check if the user wanna move in the scene
        /// </summary>
        /// <returns>The direction vector based on the user's inputs</returns>
        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            return direction;
        }

        private void ResetCameraStateTransform(SimulatorMovementComponent camComp)
        {
            camComp.m_InterpolatingCameraState.SetFromTransform(camComp.transform);
            camComp.m_TargetCameraState.SetFromTransform(camComp.transform);
        }

        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;

            foreach (var e in GetEntities<Filter>())
            {
                ResetCameraStateTransform(e.cameraComponent);
            }
        }
    }
}
