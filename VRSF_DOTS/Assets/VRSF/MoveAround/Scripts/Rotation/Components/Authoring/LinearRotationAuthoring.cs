using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
{
    /// <summary>
    /// Component used by the rotation systems to rotate the user using the thumbstick/Touchpad
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class LinearRotationAuthoring : MonoBehaviour
    {
        [Tooltip("Speed of the rotation effect when UseSmoothRotation is at true")]
        [SerializeField] private float _maxRotationSpeed = 1.0f;

        [Tooltip("How fast is the acceleration for the rotation effect going ?")]
        [Range(0.1f, 4.9f)]
        [SerializeField] private float _accelerationFactor = 1.0f;

        [Tooltip("Whether we stop the rotation abruptly or we decelerate smoothly")]
        public bool UseDecelerationEffect = true;

        [Tooltip("How fast is the deceleration for the rotation effect going ?")]
        [SerializeField]
        [HideInInspector] public float DecelerationFactor = 3.0f;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?.")]
        [SerializeField] private bool _destroyOnSceneUnloaded = true;

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(Init);
        }

        private void Init(OnSetupVRReady _)
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(Init))
                OnSetupVRReady.Listeners -= Init;

            VRInteractionAuthoring vrInteractionAuthoring = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((vrInteractionAuthoring.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture), typeof(ControllersInteractionType));

                var entity = entityManager.CreateEntity(archetype);

                Core.InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, vrInteractionAuthoring);

                entityManager.AddComponentData(entity, new LinearUserRotation
                {
                    CurrentRotationSpeed = 0.0f,
                    MaxRotationSpeed = _maxRotationSpeed,
                    AccelerationFactor = _accelerationFactor
                });

                if (UseDecelerationEffect)
                {
                    entityManager.AddComponentData(entity, new LinearRotationDeceleration
                    {
                        DecelerationFactor = DecelerationFactor
                    });
                }

                entityManager.SetComponentData(entity, new BaseInputCapture());
                entityManager.SetComponentData(entity, new TouchpadInputCapture()); 
                if (_destroyOnSceneUnloaded)
                    Core.OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "LinearRotationAuthoring");

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "User Linear Rotation Entity");
#endif
            }

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (DecelerationFactor < _accelerationFactor)
            {
                DecelerationFactor = _accelerationFactor + 0.1f;
                Debug.Log("<b>[VRSF] :</b> Deceleration Factor can't be lower than the acceleration factor, as this can cause Motion Sickness.");
            }
        }
#endif
    }
}