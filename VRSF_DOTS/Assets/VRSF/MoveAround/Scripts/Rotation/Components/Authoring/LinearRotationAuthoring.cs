using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.MoveAround.Rotation
{
    /// <summary>
    /// Component used by the rotation systems to rotate the user using the thumbstick/Touchpad
    /// </summary>
    public class LinearRotationAuthoring : MonoBehaviour
    {
        [Tooltip("How do you want to rotate ?")]
        [SerializeField] private EControllerInteractionType _interactionType = EControllerInteractionType.TOUCH;

        [Tooltip("How do you want to rotate ?")]
        [SerializeField] private EHand _hand;

        [Tooltip("Speed of the rotation effect when UseSmoothRotation is at true")]
        [SerializeField] private float _maxRotationSpeed = 1.0f;

        [Tooltip("How fast is the acceleration for the rotation effect going ?")]
        [Range(0.1f, 4.9f)]
        [SerializeField] private float _accelerationFactor = 1.0f;

        [Tooltip("Whether we stop the rotation abruptly or we decelerate smoothly")]
        public bool UseDecelerationEffect = true;

        [Tooltip("How fast is the deceleration for the rotation effect going ?")]
        [SerializeField]
        [HideInInspector] public float DecelerationFactor = 2.0f;

        public void Awake()
        {
            var entityManager = World.Active.EntityManager;

            var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture));

            var entity = entityManager.CreateEntity(archetype);

            switch (_hand)
            {
                case EHand.LEFT:
                    entityManager.AddComponentData(entity, new LeftHand());
                    break;
                case EHand.RIGHT:
                    entityManager.AddComponentData(entity, new RightHand());
                    break;
                default:
                    Debug.LogError("<b>[VRSF] :</b> Please specify a valid hand on your UserRotationAuthoring Components.");
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
            }

            entityManager.AddComponentData(entity, new UserRotationInteractionType
            {
                UseTouchToRotate = (_interactionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH,
                UseClickToRotate = (_interactionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK
            });

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