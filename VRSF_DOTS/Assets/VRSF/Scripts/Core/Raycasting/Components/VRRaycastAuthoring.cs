using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    public class VRRaycastAuthoring : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;
        
        [Header("Maximum distance of the Raycast")]
        public float MaxRaycastDistance = 200.0f;

        [Tooltip("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        public void Awake()
        {
            // We get the EntityManager
            EntityManager entityManager = World.Active.EntityManager;

            EntityArchetype archetype = entityManager.CreateArchetype
            (
                typeof(VRRaycastParameters),
                typeof(VRRaycastOrigin)
            );
            
            // We create the entity based on the archetype we juste created
            Entity raycastEntity = entityManager.CreateEntity(archetype);

            // We add the VRRaycastParameters as a struct to the newly created entity
            entityManager.SetComponentData(raycastEntity, new VRRaycastParameters
            {
                MaxRaycastDistance = MaxRaycastDistance,
                ExcludedLayer = ExcludedLayer
            });

            entityManager.SetComponentData(raycastEntity, new VRRaycastOrigin
            {
                RayOrigin = RayOrigin
            });

#if UNITY_EDITOR
            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            entityManager.SetName(raycastEntity, string.Format("Raycast Entity " + RayOrigin.ToString(), raycastEntity.Index));
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