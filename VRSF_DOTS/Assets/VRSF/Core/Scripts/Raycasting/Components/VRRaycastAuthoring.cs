using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Only use to setup the Entity you need for Raycasting in VR. This component is then destroy after being converted.
    /// </summary>
    public class VRRaycastAuthoring : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;
        
        [Header("Maximum distance of the Raycast")]
        public float MaxRaycastDistance = 200.0f;

        [Header("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        [Header("Raycast Hover Feature")]
        [Tooltip("If you want to raise an event when the pointer is hovering something.")]
        [SerializeField] private bool _useHoverFeature = true;

        [Header("Raycast Position Offset")]
        [Tooltip("If you want to apply an offset to the start point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _startPointOffset = Vector3.zero;
        [Tooltip("If you want to apply an offset to the end point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _endPointOffset = Vector3.zero;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(ConvertToEntity);
        }

        public void ConvertToEntity(OnSetupVRReady _)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity
            (
                typeof(VRRaycastParameters),
                typeof(VRRaycastOrigin),
                typeof(VRRaycastOutputs)
            );

            // We add the VRRaycastParameters as a struct to the newly created entity
            entityManager.SetComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = MaxRaycastDistance,
                ExcludedLayer = ExcludedLayer,
                StartPointOffset = _startPointOffset,
                EndPointOffset = _endPointOffset
            });

            entityManager.SetComponentData(entity, new VRRaycastOrigin
            {
                RayOrigin = RayOrigin
            });

            entityManager.SetComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable(),
                RayVar = new Ray()
            });

            if (_useHoverFeature)
                entityManager.AddComponentData(entity, new VRHovering());

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "VRRaycastAuthoring");

            var pointerClick = GetComponent<VRInteractions.PointerClickAuthoring>();
            if (pointerClick != null)
            {
                pointerClick.AddPointerClickComponents(entity);
                if (GetComponent<LineRenderer>() != null)
                    Destroy(this);
                else
                    Destroy(gameObject);
            }

            Destroy(this);
        }

        public Vector3 StartPointOffset { get => _startPointOffset; }
        public Vector3 EndPointOffset { get => _endPointOffset; }
    }
}