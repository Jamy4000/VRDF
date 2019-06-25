﻿using Unity.Entities;

namespace VRSF.Core.Raycast
{
    public struct VRRaycastOrigin : IComponentData
    {
        /// <summary>
        /// The origin of the raycast for this component
        /// </summary>
        public ERayOrigin RayOrigin;

        /// <summary>
        /// The position of the origin for this ray
        /// </summary>
        public Unity.Mathematics.float3 RayOriginPosition;
    }
}