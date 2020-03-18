using Unity.Entities;
using UnityEngine;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// Calculate and move the Simulator in the scene
    /// </summary>
    public class SimulatorHorizontalMover : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Vector3 translation = GetInputTranslationDirection();
            bool isUsingShiftBoost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            Entities.ForEach((ref SimulatorAcceleration acceleration, ref SimulatorMovements movements) =>
            {
                if (translation != Vector3.zero)
                {
                    translation *= isUsingShiftBoost ? movements.WalkSpeed * movements.ShiftBoost : movements.WalkSpeed;

                    if (acceleration.AccelerationTimer < acceleration.MaxAccelerationFactor)
                        acceleration.AccelerationTimer += Time.DeltaTime / acceleration.AccelerationSpeed;

                    translation *= acceleration.AccelerationTimer;

                    var posBeforeMoving = VRDF_Components.CameraRig.transform.position;

                    VRDF_Components.CameraRig.transform.position += VRDF_Components.VRCamera.transform.TransformDirection(translation);

                    if (movements.IsGrounded)
                        CheckForGround(posBeforeMoving);
                }
                else if (acceleration.AccelerationTimer > 0.0f)
                {
                    acceleration.AccelerationTimer -= Time.DeltaTime / acceleration.AccelerationSpeed;
                }
            });
        }

        /// <summary>
        /// Get inputs to check if the user wanna move in the scene
        /// </summary>
        /// <returns>The direction vector based on the user's inputs</returns>
        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // Forward or backward
            if (vertical != 0.0f)
                direction.z += vertical;

            // left or right
            if (horizontal != 0.0f)
                direction.x += horizontal;

            return direction;
        }

        private void CheckForGround(Vector3 posBeforeMoving)
        {
            if (Physics.Raycast(new Ray(VRDF_Components.VRCamera.transform.position, Vector3.down), out RaycastHit hit, 100.0f))
            {
                var currentPos = VRDF_Components.CameraRig.transform.position;
                VRDF_Components.CameraRig.transform.position = new Vector3
                (
                    currentPos.x,
                    hit.point.y,
                    currentPos.z
                );
            }
            else
            {
                VRDF_Components.CameraRig.transform.position = posBeforeMoving;
            }
        }
    }
}