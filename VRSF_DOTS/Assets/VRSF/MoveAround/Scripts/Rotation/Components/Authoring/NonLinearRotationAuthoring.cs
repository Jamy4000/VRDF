using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;

namespace VRSF.MoveAround.Rotation
{
    /// <summary>
    /// Component used by the rotation systems to rotate the user using the thumbstick/Touchpad
    /// </summary>
    public class NonLinearRotationAuthoring : MonoBehaviour
    {
        [Tooltip("How do you want to rotate ?")]
        [SerializeField] private EControllerInteractionType _interactionType = EControllerInteractionType.TOUCH;

        [Tooltip("How do you want to rotate ?")]
        [SerializeField] private EHand _hand;

        [Tooltip("Amount of degrees to rotate when UseSmoothRotation is at false")]
        [SerializeField] private float _degreesToRotate = 30.0f;

        public void Awake()
        {
            var entityManager = World.Active.EntityManager;

            var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture), typeof(ControllersInteractionType));

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

            entityManager.AddComponentData(entity, new ControllersInteractionType
            {
                InteractionType = _interactionType,
                HasTouchInteraction = (_interactionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH,
                HasClickInteraction = (_interactionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK
            });

            entityManager.AddComponentData(entity, new NonLinearUserRotation { DegreesToRotate = this._degreesToRotate });

            entityManager.SetComponentData(entity, new BaseInputCapture());
            entityManager.SetComponentData(entity, new TouchpadInputCapture());

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "User Non Linear Rotation Entity");
#endif

            Destroy(gameObject);
        }
    }
}