using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Simulator
{
    public class SimulatorRotationAuthoring : MonoBehaviour
    {
        [Header("Base speed for rotating")]
        [SerializeField]
        private float _rotationSpeed = 1.0f;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?.")]
        [SerializeField] private bool _destroyOnSceneUnloaded = true;

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(ConvertToEntity);
        }

        private void ConvertToEntity(OnSetupVRReady _)
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(ConvertToEntity))
                OnSetupVRReady.Listeners -= ConvertToEntity;

            if (VRSF_Components.DeviceLoaded != SetupVR.EDevice.SIMULATOR)
            {
                Destroy(this);
                return;
            }

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity
            (
                typeof(SimulatorRotation),
                typeof(DestroyOnSceneUnloaded)
            );

            entityManager.SetComponentData(entity, new SimulatorRotation
            {
                RotationSpeed = _rotationSpeed
            });

            if (_destroyOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "SimulatorRotationAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Simulator Rotation Entity");
#endif

            Destroy(this);
        }
    }
}