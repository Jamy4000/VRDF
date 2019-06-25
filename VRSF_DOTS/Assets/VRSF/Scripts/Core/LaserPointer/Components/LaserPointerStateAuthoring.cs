using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    [RequiresEntityConversion]
    public class LaserPointerStateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Laser Renderering Parameters")]
        [Tooltip("The prefab for the line renderer in ECS.")]
        public GameObject LineRendererPrefab;
        [Tooltip("The base width for this pointer when you are pointing at something.")]
        public float PointerWidth = 0.01f;

        [Header("Disappearance Parameters")]
        [Tooltip("The base state of the Pointer.")]
        public EPointerState BaseState = EPointerState.ON;
        [Tooltip("How fast the pointer is disappearing when not hitting something. Set it to zero to stop the fade out of the laser.")]
        public float DisappearanceSpeed = 1.0f;

        private void Awake()
        {
            
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new LaserPointerState
            {
                State = BaseState
            });

            dstManager.AddComponentData(entity, new LaserPointerVisibility
            {
                DisappearanceSpeed = DisappearanceSpeed
            });

            dstManager.AddComponentData(entity, new LaserPointerWidth
            {
                CurrentWidth = PointerWidth,
                BaseWidth = PointerWidth
            });

            float maxDistance = GetComponent<VRRaycastAuthoring>().MaxRaycastDistance;

            dstManager.AddComponentData(entity, new LaserPointerLength
            {
                CurrentLength = maxDistance,
                BaseLength = maxDistance
            });

            var drawer = gameObject.AddComponent<LaserPointerDrawer>();
            drawer.LineZPosition = (int)maxDistance;

            Destroy(this);
        }
    }

    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponentTag(typeof(VRRaycastOrigin))]
    public struct LaserPointerState : IComponentData
    {
        /// <summary>
        /// The current state of the Pointer.
        /// </summary>
        public EPointerState State;
    }
}