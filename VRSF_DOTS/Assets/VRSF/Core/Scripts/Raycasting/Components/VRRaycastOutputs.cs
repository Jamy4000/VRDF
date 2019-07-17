using UnityEngine;
using Unity.Entities;

namespace VRSF.Core.Raycast
{
    public struct VRRaycastOutputs : IComponentData
    {
        /// <summary>
        /// Reference to the RaycastHit for when this Controller's ray is hitting something
        /// </summary>
        public RaycastHitVariable RaycastHitVar;

        /// <summary>
        /// Reference to the Ray starting from this Controller
        /// </summary>
        public Ray RayVar;
    }
}