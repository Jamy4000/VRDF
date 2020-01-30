using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.Utils;

namespace VRSF.Core.LaserPointer
{
    public class LaserPointerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Laser Renderering Parameters")]
        [Tooltip("The base width for this pointer when you are pointing at something.")]
        [SerializeField] private float _pointerWidth = 0.001f;
        [Tooltip("If you want the pointer to set its endpoint to the center of the 3D object it just hit.")]
        [SerializeField] private bool _shouldPointTo3DObjectCenter;
        [Tooltip("If you want the pointer to set its endpoint to the center of the UI element it just hit. WARNING : UI Colliders needs to be on the UI Layer.")]
        [SerializeField] private bool _shouldPointToUICenter;

        [Header("Disappearance Parameters")]
        [Tooltip("The base state of the Pointer.")]
        [SerializeField] private EPointerState _baseState = EPointerState.ON;
        [Tooltip("How fast the pointer is disappearing when not hitting something. Set it to zero to stop the fade out of the laser.")]
        [SerializeField] private float _disappearanceSpeed = 1.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new LaserPointerState
            {
                State = _baseState,
                StateJustChangedToOn = false
            });

            dstManager.AddComponentData(entity, new LaserPointerVisibility
            {
                DisappearanceSpeed = _disappearanceSpeed
            });

            dstManager.AddComponentData(entity, new LaserPointerWidth
            {
                BaseWidth = _pointerWidth
            });

            VRRaycastAuthoring raycastAuthoring = GetComponent<VRRaycastAuthoring>();

            dstManager.AddComponentData(entity, new LaserPointerLength
            {
                BaseLength = raycastAuthoring.MaxRaycastDistance,
                ShouldPointTo3DObjectsCenter = _shouldPointTo3DObjectCenter,
                ShouldPointToUICenter = _shouldPointToUICenter
            });

#if UNITY_EDITOR
            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            dstManager.SetName(entity, string.Format("Laser Pointer {0}", raycastAuthoring.RayOrigin.ToString()));
#endif

            Destroy(GetComponent<ConvertToEntity>());
            Destroy(this);
        }
    }
}