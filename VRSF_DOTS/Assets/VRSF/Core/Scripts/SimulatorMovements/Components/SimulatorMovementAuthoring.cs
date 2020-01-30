using E7.DataStructure;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;

namespace VRSF.Core.Simulator
{
    public class SimulatorMovementAuthoring : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Base Speed of the camera movements.")]
        [Range(1, 10)]
        [SerializeField] private float _baseSpeed = 1.0f;

        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [Range(1, 10)]
        [SerializeField] private float _leftShiftBoost = 2.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float _positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField] private AnimationCurve _mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 1.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float _rotationLerpTime = 0.01f;

        [Tooltip("Whether the Rotation of the camera in the Y axis should be inversed")]
        [SerializeField] private bool _reversedYAxis = true;

        public void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(Init);
        }

        public void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(Init))
                OnSetupVRReady.Listeners -= Init;
        }

        private void Init(OnSetupVRReady info)
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(SimulatorMovementSpeed),
                    typeof(SimulatorMovementRotation),
                    typeof(JobAnimationCurve),
                    typeof(SimulatorCameraState),
                    typeof(DestroyOnSceneUnloaded)
                );

                var entity = entityManager.CreateEntity(archetype);

                entityManager.SetComponentData(entity, new SimulatorMovementSpeed
                {
                    BaseShiftBoost = _leftShiftBoost,
                    BaseSpeed = _baseSpeed * 2,
                    PositionLerpTime = _positionLerpTime
                });

                entityManager.SetComponentData(entity, new SimulatorMovementRotation
                {
                    ReversedYAxis = _reversedYAxis,
                    RotationLerpTime = _rotationLerpTime
                });

                entityManager.SetComponentData(entity, new SimulatorCameraState()
                {
                    InterpolatingCameraState = new CameraState(),
                    TargetCameraState = new CameraState()
                });

                entityManager.SetComponentData(entity, new JobAnimationCurve(_mouseSensitivityCurve, Unity.Collections.Allocator.Persistent));

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "Simulator Movements Entity");
#endif
            }

            Destroy(this);
        }
    }
}
