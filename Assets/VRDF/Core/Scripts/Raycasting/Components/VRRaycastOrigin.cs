﻿using Unity.Entities;

namespace VRDF.Core.Raycast
{
    public struct VRRaycastOrigin : IComponentData
    {
        /// <summary>
        /// The origin of the raycast for this component
        /// </summary>
        public ERayOrigin RayOrigin;
    }
}