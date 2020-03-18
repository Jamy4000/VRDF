using Unity.Entities;
using UnityEngine;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// Calculate and rotate the Simulator Camera in the scene
    /// </summary>
    public class SimulatorRotator : ComponentSystem
    {
        private Transform _vrCamera;

        protected override void OnCreate()
        {
            base.OnCreate();
            OnSetupVRReady.Listeners += AssignToRotate;
        }

        protected override void OnUpdate()
        {
            CheckCursorState();
            Vector3 rotation = GetInputRotationDirection();

            Entities.ForEach((ref SimulatorRotation rotationComp) =>
            {
                if (rotation != Vector3.zero)
                {
                    rotation *= rotationComp.RotationSpeed;

                    var newRotation = new Vector3
                    {
                        x = _vrCamera.eulerAngles.x + rotation.x,
                        y = _vrCamera.eulerAngles.y + rotation.y,
                        z = 0.0f
                    };

                    _vrCamera.rotation = Quaternion.Euler(newRotation);
                }
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= AssignToRotate;
        }

        private Vector3 GetInputRotationDirection()
        {
            // Getting Input and Reversing Y Axis
            Vector2 mouseMovements = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

            // Rotation with right click
            if (Input.GetMouseButton(1))
            {
                // I know, mouseMovements axis are inverted, but it works.
                // No Idea why, but here it is, perfectly working, so let it like that PLEASE
                float yaw = mouseMovements.y * 2.0f;
                float pitch = mouseMovements.x * 2.0f;

                return new Vector3(yaw, pitch, 0.0f);
            }
            else
            {
                return Vector3.zero;
            }
        }

        private void CheckCursorState()
        {
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
        }

        private void AssignToRotate(OnSetupVRReady _)
        {
            _vrCamera = VRDF_Components.VRCamera.transform;
        }
    }
}