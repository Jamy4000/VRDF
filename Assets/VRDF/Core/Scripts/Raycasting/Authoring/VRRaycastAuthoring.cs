using Unity.Entities;
using UnityEngine;

namespace VRDF.Core.Raycast
{
    /// <summary>
    /// Only use to setup the Entity you need for Raycasting in VR. 
    /// This component is then destroy after being converted.
    /// </summary>
    public class VRRaycastAuthoring : MonoBehaviour
    {
        [Header("General Raycast Parameters")]
        [Tooltip("The Raycast Origin for this script")]
        [SerializeField] private ERayOrigin _rayOrigin = ERayOrigin.NONE;
        [Tooltip("Maximum distance of the Raycast")]
        [SerializeField] private float _maxRaycastDistance = 200.0f;
        [Tooltip("Layer(s) to exclude from the Raycast System.")]
        [SerializeField] private LayerMask _excludedLayer = new LayerMask();

        [Header("Additional Parameters")]
        [Tooltip("If you want to raise the 'ObjectIsBeingHoveredEvent' event when the pointer is hovering something.")]
        [SerializeField] private bool _useHoverFeature = true;
        [Tooltip("If you want to apply an offset to the start point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _startPointOffset = Vector3.zero;
        [Tooltip("If you want to apply an offset to the end point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _endPointOffset = Vector3.zero;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        [HideInInspector] public bool CheckForVRRaycaster = true;

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(ConvertToEntity);
        }

        /// <summary>
        /// Create the Entity
        /// </summary>
        /// <param name="_"></param>
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
                MaxRaycastDistance = _maxRaycastDistance,
                ExcludedLayer = _excludedLayer,
                StartPointOffset = _startPointOffset,
                EndPointOffset = _endPointOffset
            });

            entityManager.SetComponentData(entity, new VRRaycastOrigin
            {
                RayOrigin = RayOrigin
            });

            entityManager.SetComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable { IsNull = true },
                RayVar = new Ray()
            });

            // If we use the Hovering feature, we then add a VRHovering tag
            if (_useHoverFeature)
                entityManager.AddComponentData(entity, new VRHovering());

            // If we want to destroy this entity when a new scene is loaded
            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(ref entityManager, ref entity, gameObject.scene.buildIndex, "VRRaycastAuthoring");

            // If this gameObject has a PointerClickAuthoring, we add the corresponding component to the entity
            var pointerClick = GetComponent<VRClicker.VRClickerAuthoring>();
            if (pointerClick != null)
                pointerClick.AddPointerClickComponents(ref entity, ref entityManager);

            // If this gameObject has a LaserPointerAuthoring, we add the corresponding component to the entity
            var laserPointerAuthoring = GetComponent<LaserPointer.LaserPointerAuthoring>();
            if (laserPointerAuthoring != null)
                laserPointerAuthoring.AddLaserPointerComponents(ref entity, ref entityManager, this);

            // in the case we're using a laser with LineRenderer, we just destroy this script. If not, we destroy the gameObject
            if (GetComponent<LineRenderer>() != null)
                Destroy(this);
            else
                Destroy(gameObject);
        }

        public Vector3 StartPointOffset { get => _startPointOffset; }
        public float MaxRaycastDistance { get => _maxRaycastDistance; }
        public ERayOrigin RayOrigin { get => _rayOrigin; }


        public static void CheckSceneContainsRaycaster()
        {
            if (GameObject.FindObjectsOfType<Core.Raycast.VRRaycastAuthoring>().Length == 0)
                Debug.LogError("<Color=red><b>[VRDF]:</b> You need at least one VRRaycastAuthoring in your scene to initialize and use the VR UI Package.\n" +
                    "To do so, Right Click in Hierarchy > VRDF > Raycaster, Laser Pointer and VR Clicker.</Color>");
        }
    }
}