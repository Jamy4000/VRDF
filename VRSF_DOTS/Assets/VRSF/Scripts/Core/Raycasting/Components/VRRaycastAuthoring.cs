using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
    [RequiresEntityConversion]
    public class VRRaycastAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;
        
        [Header("Maximum distance of the Raycast")]
        public float MaxRaycastDistance = 200.0f;

        [Header("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // We add the VRRaycastParameters as a struct to the newly created entity
            dstManager.AddComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = MaxRaycastDistance,
                ExcludedLayer = ExcludedLayer
            });

            dstManager.AddComponentData(entity, new VRRaycastOrigin
            {
                RayOrigin = RayOrigin
            });

#if UNITY_EDITOR
            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            dstManager.SetName(entity, string.Format("Raycast " + RayOrigin.ToString(), entity.Index));
#endif

            // We destroy this component as we don't need it anymore
            Destroy(this);
        }
    }
    
    public struct VRRaycastParameters : IComponentData
    {
        /// <summary>
        /// The Maximum distance of the Raycast
        /// </summary>
        public float MaxRaycastDistance;

        /// <summary>
        /// Layer(s) to exclude from the Raycast System.
        /// </summary>
        public LayerMask ExcludedLayer;
    }
}