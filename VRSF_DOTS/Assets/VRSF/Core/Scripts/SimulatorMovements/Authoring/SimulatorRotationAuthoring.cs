using Unity.Entities;
using UnityEngine;
using VRSF.Core.Utils;

namespace VRSF.Core.Simulator
{
    public class SimulatorRotationAuthoring : MonoBehaviour
    {
        [Header("Base speed for rotating")]
        [SerializeField]
        private float _rotationSpeed = 1.0f;

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

            entityManager.SetComponentData(entity, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Simulator Rotation Entity");
#endif

            Destroy(this);
        }
    }
}