using E7.ECS.LineRenderer;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    public class LaserPointerStateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Laser Renderering Parameters")]
        [Tooltip("The material used for the line renderer in ECS.")]
        public Material LineRendererMaterial;
        [Tooltip("The base width for this pointer when you are pointing at something.")]
        public float PointerWidth = 0.01f;

        [Header("Disappearance Parameters")]
        [Tooltip("The base state of the Pointer.")]
        public EPointerState BaseState = EPointerState.ON;
        [Tooltip("How fast the pointer is disappearing when not hitting something. Set it to zero to stop the fade out of the laser.")]
        public float DisappearanceSpeed = 1.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.RemoveComponent(entity, ComponentType.ReadOnly<Translation>());
            dstManager.RemoveComponent(entity, ComponentType.ReadOnly<Rotation>());
            dstManager.RemoveComponent(entity, ComponentType.ReadOnly<NonUniformScale>());
            dstManager.RemoveComponent(entity, ComponentType.ReadOnly<LocalToWorld>());

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
                BaseWidth = PointerWidth
            });

            VRRaycastAuthoring raycastAuthoring = GetComponent<VRRaycastAuthoring>();

            dstManager.AddComponentData(entity, new LaserPointerLength
            {
                BaseLength = raycastAuthoring.MaxRaycastDistance
            });

            dstManager.AddComponentData(entity, new LineSegment
            (
                transform.position,
                new float3(0, 0, raycastAuthoring.MaxRaycastDistance),
                PointerWidth
            ));

#if UNITY_EDITOR
            if (LineRendererMaterial == null)
                LineRendererMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/ECSLineRenderer/SampleLineMaterial.mat");

            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            dstManager.SetName(entity, string.Format("Laser Pointer " + raycastAuthoring.RayOrigin.ToString(), entity.Index));
#endif

            dstManager.AddSharedComponentData(entity, new LineStyle
            {
                material = LineRendererMaterial
            });

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