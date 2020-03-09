using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Simulator
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

            Entities.ForEach((ref SimulatorAcceleration acceleration, ref SimulatorMovements horizontalMovements) =>
            {
                if (translation != Vector3.zero)
                {
                    translation *= isUsingShiftBoost ? horizontalMovements.WalkSpeed * horizontalMovements.ShiftBoost : horizontalMovements.WalkSpeed;

                    if (acceleration.AccelerationTimer < acceleration.MaxAccelerationFactor)
                        acceleration.AccelerationTimer += Time.DeltaTime / acceleration.AccelerationSpeed;

                    translation *= acceleration.AccelerationTimer;
                    VRSF_Components.CameraRig.transform.Translate(translation);
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
    }
}