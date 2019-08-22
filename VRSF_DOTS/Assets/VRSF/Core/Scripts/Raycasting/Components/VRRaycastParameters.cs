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

        /// <summary>
        /// If you want to apply an offset to the start point of the raycast. This will as well be applied to the laser if you use one.
        /// </summary>
        public Vector3 StartPointOffset;

        /// <summary>
        /// If you want to apply an offset to the end point of the raycast. This will as well be applied to the laser if you use one.
        /// </summary>
        public Vector3 EndPointOffset;
    }
}