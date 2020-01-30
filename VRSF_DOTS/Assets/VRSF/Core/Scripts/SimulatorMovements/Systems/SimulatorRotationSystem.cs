using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;
using E7.DataStructure;
using Unity.Jobs;
using Unity.Collections;

namespace VRSF.Core.Simulator
{
    [UpdateBefore(typeof(SimulatorRotationInterpolationSystem))]
    public class SimulatorRotationSystem : JobComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!VRSF_Components.SetupVRIsReady)
                return inputDeps;

            // Deactivate cursor visibility and lock it when starting to rotate
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            // Reactivate cursor visibility and delock it when stopping to rotate
            else if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            return new SimulatorCameraRotationJob
            {
                MouseMovements = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")),
                RightClickIsClicking = Input.GetMouseButton(1),
                DeltaTime = Time.DeltaTime
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        private struct SimulatorCameraRotationJob : IJobForEach<SimulatorMovementRotation, SimulatorMovementSpeed, SimulatorCameraState, JobAnimationCurve>
        {
            public bool RightClickIsClicking;
            public Vector2 MouseMovements;
            public float DeltaTime;

            public void Execute(ref SimulatorMovementRotation smr, ref SimulatorMovementSpeed sms, ref SimulatorCameraState scs, ref JobAnimationCurve jac)
            {
                // Rotation with right click
                if (RightClickIsClicking)
                {
                    if (smr.ReversedYAxis)
                        MouseMovements.y = -MouseMovements.y;

                    // Evaluate the camera rotation based on the mouse screen position
                    var mouseSensitivityFactor = jac.Evaluate(DeltaTime);//mouseMovements.magnitude);
                    scs.TargetCameraState.yaw += MouseMovements.x * mouseSensitivityFactor;
                    scs.TargetCameraState.pitch += MouseMovements.y * mouseSensitivityFactor;
                }
            }
        }

        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}
