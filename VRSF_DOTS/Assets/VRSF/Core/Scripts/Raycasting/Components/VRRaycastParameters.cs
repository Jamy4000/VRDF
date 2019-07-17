using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Raycast
{
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