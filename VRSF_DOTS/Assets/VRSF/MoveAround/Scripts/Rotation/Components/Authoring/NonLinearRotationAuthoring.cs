using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Utils;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
{
    /// <summary>
    /// Component used by the rotation systems to rotate the user using the thumbstick/Touchpad
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class NonLinearRotationAuthoring : MonoBehaviour
    {
        [Tooltip("Amount of degrees to rotate when UseSmoothRotation is at false")]
        [SerializeField] private float _degreesToRotate = 30.0f;

        private void Awake()
        {
            VRInteractionAuthoring vrInteractionAuthoring = GetComponent<VRInteractionAuthoring>();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture), typeof(ControllersInteractionType));

            var entity = entityManager.CreateEntity(archetype);

            InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, vrInteractionAuthoring);

            entityManager.AddComponentData(entity, new NonLinearUserRotation { DegreesToRotate = this._degreesToRotate });

            entityManager.AddComponentData(entity, new DestroyOnSceneUnloaded());

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "User Non Linear Rotation Entity");
#endif

            Destroy(gameObject);
        }
    }
}