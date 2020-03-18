using Unity.Entities;
using UnityEngine;
using VRDF.Core.Inputs;
using VRDF.Core.VRInteractions;

namespace VRDF.MoveAround.VRRotation
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
        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        private void Awake()
        {
            // TODO Remove Simulator from this VRInteractionAuthoring and display a HelpBox telling that, to rotate with simulator, it's better to use the Simulator Rotation Scripts
            OnSetupVRReady.RegisterSetupVRCallback(Init);
        }

        private void Init(OnSetupVRReady _)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(Init);

            VRInteractionAuthoring vrInteractionAuthoring = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((vrInteractionAuthoring.DeviceUsingFeature & VRDF_Components.DeviceLoaded) == VRDF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var entity = entityManager.CreateEntity
                (
                    typeof(BaseInputCapture), 
                    typeof(TouchpadInputCapture), 
                    typeof(ControllersInteractionType),
                    typeof(LinearUserRotation)
                );


                // Add the corresponding input, Hand and Interaction type component for the selected button. 
                // If the button wasn't chose correctly or any parameter was wrongly set, we destroy this entity and return.
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, vrInteractionAuthoring))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

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

                if (_destroyEntityOnSceneUnloaded)
                    Core.OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(ref entityManager, ref entity, gameObject.scene.buildIndex, "LinearRotationAuthoring");

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
                Debug.Log("<b>[VRDF] :</b> Deceleration Factor can't be lower than the acceleration factor, as this can cause Motion Sickness.");
            }

            GetComponent<VRInteractionAuthoring>().ButtonToUse = EControllersButton.TOUCHPAD;
        }
#endif
    }
}