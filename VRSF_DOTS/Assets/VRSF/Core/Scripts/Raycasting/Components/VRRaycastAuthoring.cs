using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Only use to setup the Entity you need for Raycasting in VR. This component is then destroy after being converted.
    /// </summary>
    [RequiresEntityConversion]
    public class VRRaycastAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;
        
        [Header("Maximum distance of the Raycast")]
        public float MaxRaycastDistance = 200.0f;

        [Header("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        [Header("Raycast Position Offset")]
        [Tooltip("If you want to apply an offset to the start point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _startPointOffset = Vector3.zero;
        [Tooltip("If you want to apply an offset to the end point of the raycast. This will as well be applied to the laser if you use one.")]
        [SerializeField] private Vector3 _endPointOffset = Vector3.zero;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // We add the VRRaycastParameters as a struct to the newly created entity
            dstManager.AddComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = MaxRaycastDistance,
                ExcludedLayer = ExcludedLayer,
                StartPointOffset = _startPointOffset,
                EndPointOffset = _endPointOffset
            });

            dstManager.AddComponentData(entity, new VRRaycastOrigin
            {
                RayOrigin = RayOrigin
            });

            dstManager.AddComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable(),
                RayVar = new Ray()
            });

            Destroy(this);
        }
    }
}