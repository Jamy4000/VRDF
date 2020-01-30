using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.Utils;
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

        public void Awake()
        {
            VRInteractionAuthoring vrInteractionAuthoring = GetComponent<VRInteractionAuthoring>();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture), typeof(ControllersInteractionType));

            var entity = entityManager.CreateEntity(archetype);

            InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, vrInteractionAuthoring);

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
            entityManager.AddComponentData(entity, new DestroyOnSceneUnloaded());

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "User Linear Rotation Entity");
#endif

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