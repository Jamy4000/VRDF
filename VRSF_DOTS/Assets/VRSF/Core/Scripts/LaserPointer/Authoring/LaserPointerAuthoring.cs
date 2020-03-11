using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// 
    /// </summary>
    public class LaserPointerAuthoring : MonoBehaviour
    {
        [Header("Laser Renderering Parameters")]
        [Tooltip("The base width for this pointer when you are pointing at something.")]
        [SerializeField] private float _pointerWidth = 0.001f;
        [Tooltip("If you want the pointer to set its endpoint to the center of the 3D object it just hit.")]
        [SerializeField] private bool _shouldPointTo3DObjectCenter = false;
        [Tooltip("If you want the pointer to set its endpoint to the center of the UI element it just hit. WARNING : UI Colliders needs to be on the UI Layer.")]
        [SerializeField] private bool _shouldPointToUICenter = false;

        [Header("Disappearance Parameters")]
        [Tooltip("The base state of the Pointer.")]
        [SerializeField] private EPointerState _baseState = EPointerState.ON;
        [Tooltip("How fast the pointer is disappearing when not hitting something. Set it to zero to stop the fade out of the laser.")]
        [SerializeField] private float _disappearanceSpeed = 1.0f;

        /// <summary>
        /// Add all components required to calculate the Laser Pointer position and width
        /// </summary>
        /// <param name="entity">the entity to which we want to add the components</param>
        /// <param name="entityManager">The entity manager, so we don't have to fetch it again</param>
        /// <param name="raycastAuthoring">The raycast authoring to set the base distance of the line</param>
        public void AddLaserPointerComponents(ref Entity entity, ref EntityManager entityManager, VRRaycastAuthoring raycastAuthoring)
        {
            entityManager.AddComponentData(entity, new LaserPointerState
            {
                State = _baseState,
                StateJustChangedToOn = false
            });

            if (_disappearanceSpeed > 0.0f)
            {
                entityManager.AddComponentData(entity, new LaserPointerVisibility
                {
                    DisappearanceSpeed = _disappearanceSpeed,
                    BaseWidth = _pointerWidth,
                    CurrentWidth = _pointerWidth
                });
            }

            entityManager.AddComponentData(entity, new LaserPointerLength
            {
                BaseLength = raycastAuthoring.MaxRaycastDistance,
                ShouldPointTo3DObjectsCenter = _shouldPointTo3DObjectCenter,
                ShouldPointToUICenter = _shouldPointToUICenter
            });

            // Check if a line renderer is already present on the gameObject, and if not, add one
            if (GetComponent<LineRenderer>() == null)
            {
                var lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = _pointerWidth;
                lineRenderer.endWidth = _pointerWidth;
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.white;
            }

            // Check if a LaserLengthSetter is already present on the gameObject, and if not, add one
            if (GetComponent<LaserLengthSetter>() == null)
                gameObject.AddComponent<LaserLengthSetter>();

            // Check if a LaserWidthSetter is already present on the gameObject, and if not, add one
            if (GetComponent<LaserWidthSetter>() == null)
                gameObject.AddComponent<LaserWidthSetter>();


#if UNITY_EDITOR
            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, string.Format("Laser Pointer {0}", raycastAuthoring.RayOrigin.ToString()));
#endif

            Destroy(this);
        }
    }
}