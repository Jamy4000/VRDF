using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Create an entity to rotate the Simulator in the Scene using the Right Click
    /// </summary>
    public class SimulatorRotationAuthoring : MonoBehaviour
    {
        [Header("Base speed for rotating")]
        [SerializeField]
        private float _rotationSpeed = 1.0f;

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

            // If we're not using the simulator, no need for a simulator rotation entity
            if (VRSF_Components.DeviceLoaded != SetupVR.EDevice.SIMULATOR)
            {
                Destroy(this);
                return;
            }

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity
            (
                typeof(SimulatorRotation)
            );

            entityManager.SetComponentData(entity, new SimulatorRotation
            {
                RotationSpeed = _rotationSpeed
            });

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(ref entityManager, ref entity, gameObject.scene.buildIndex, "SimulatorRotationAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "Simulator Rotation Entity");
#endif

            Destroy(this);
        }
    }
}