using UnityEngine;

namespace VRSF.Core.Inputs
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SimulatorMovementComponent : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Base Speed of the camera movements.")]
        [Range(1, 10)]
        public float BaseSpeed = 1.0f;

        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [Range(1, 10)]
        [SerializeField] private float _leftShiftBoost = 2.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float PositionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve MouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 1.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float RotationLerpTime = 0.01f;

        [Tooltip("Whether the Rotation of the camera in the Y axis should be inversed")]
        public bool ReversedYAxis = true;

        [HideInInspector]
        public CameraState m_TargetCameraState = new CameraState();
        [HideInInspector]
        public CameraState m_InterpolatingCameraState = new CameraState();

        public float LeftShiftBoost
        {
            get => _leftShiftBoost;
            set
            {
                _leftShiftBoost = value;

                if (_leftShiftBoost > 5)
                    _leftShiftBoost = 5;
                else if (_leftShiftBoost < 1)
                    _leftShiftBoost = 1;
            }
        }
    }
}
