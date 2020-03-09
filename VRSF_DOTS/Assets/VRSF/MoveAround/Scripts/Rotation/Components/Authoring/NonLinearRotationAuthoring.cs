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
    public class NonLinearRotationAuthoring : MonoBehaviour
    {
        [Tooltip("Amount of degrees to rotate when UseSmoothRotation is at false")]
        [SerializeField] private float _degreesToRotate = 30.0f;

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
            if ((vrInteractionAuthoring.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype(typeof(BaseInputCapture), typeof(TouchpadInputCapture), typeof(ControllersInteractionType));

                var entity = entityManager.CreateEntity(archetype);

                InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, vrInteractionAuthoring);

                entityManager.AddComponentData(entity, new NonLinearUserRotation { DegreesToRotate = this._degreesToRotate });

                if (_destroyEntityOnSceneUnloaded)
                    Core.OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(ref entityManager, ref entity, gameObject.scene.buildIndex, "NonLinearRotationAuthoring");

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "User Non Linear Rotation Entity");
#endif
            }

            Destroy(gameObject);
        }
    }
}