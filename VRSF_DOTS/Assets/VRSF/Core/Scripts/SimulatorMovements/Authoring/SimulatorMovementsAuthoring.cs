using UnityEngine;
using Unity.Entities;

namespace VRSF.Core.Simulator
{
    public class SimulatorMovementsAuthoring : MonoBehaviour
    {
        [Header("Base speed and boost for moving on the Horizontal (x and z) axis")]
        [SerializeField]
        private float _walkSpeed = 1.0f;
        [SerializeField]
        private float _shiftBoost = 3.0f;

        [Header("Acceleration factors for smooth movements")]
        [Tooltip("Maximum acceleration factor to apply to the base speed.")]
        [SerializeField]
        private float _maxAccelerationFactor = 2.0f;
        [Tooltip("How long the acceleration effect nees to go full speed. The higher the number is, the slower you gonna accelerate.")]
        [SerializeField]
        private float _accelerationSpeed = 3.0f;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(ConvertToEntity);
        }

        private void ConvertToEntity(OnSetupVRReady _)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(ConvertToEntity);

            if (VRSF_Components.DeviceLoaded != SetupVR.EDevice.SIMULATOR)
            {
                Destroy(this);
                return;
            }

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity
            (
                typeof(SimulatorMovements),
                typeof(SimulatorAcceleration),
                typeof(DestroyOnSceneUnloaded)
            );

            entityManager.SetComponentData(entity, new SimulatorMovements
            {
                WalkSpeed = _walkSpeed * 0.1f,
                ShiftBoost = _shiftBoost
            });

            entityManager.SetComponentData(entity, new SimulatorAcceleration
            {
                AccelerationSpeed = _accelerationSpeed,
                MaxAccelerationFactor = _maxAccelerationFactor,
                AccelerationTimer = 0.0f
            });

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "SimulatorMovementsAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Simulator Movements Entity");
#endif

            Destroy(this);
        }
    }
}