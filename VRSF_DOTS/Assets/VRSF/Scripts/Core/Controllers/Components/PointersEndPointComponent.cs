using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// A component to add an End Point feature on the pointer.
    /// Usefull as sometimes the laser can be quite small and it can be difficult to see where the user is pointing at.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class PointersEndPointComponent : MonoBehaviour
    {
        [Tooltip("The Scriptable Raycast that should be place on the parent of this object.")]
        public ScriptableRaycastComponent ParentRaycastComponent;

        [Tooltip("The RaycastHit Variable corresponding to where we need to place the end point")]
        public RaycastHitVariable HitVariable;
    }
} 