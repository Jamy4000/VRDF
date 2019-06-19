using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Raycast
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ScriptableRaycastComponent : MonoBehaviour
    {
        [Header("The Raycast Origin for this script")]
        public ERayOrigin RayOrigin = ERayOrigin.NONE;
        
        [Header("Maximum distance of the Raycast")]
        public float MaxRaycastDistance;

        [Tooltip("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        /// <summary>
        /// The Transform of the origin from the Ray
        /// </summary>
        [HideInInspector] public Transform RayOriginTransform;

        /// <summary>
        /// Reference to the RaycastHitVariable link to this hand
        /// </summary>
        [HideInInspector] public RaycastHitVariable RaycastHitVar;

        /// <summary>
        /// Reference to the RayVariable link to this hand
        /// </summary>
        [HideInInspector] public RayVariable RayVar;

        /// <summary>
        /// Reference to the VRCamera object
        /// </summary>
        [HideInInspector] public Camera _VRCamera;

        /// <summary>
        /// Wheter we check the raycast, set at runtime by checking if we use the controllers or the gaze
        /// </summary>
        [HideInInspector] public bool IsSetup = false;
    }
}