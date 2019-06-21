using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;
using Unity.Mathematics;

namespace VRSF.Core.Raycast
{
    public struct VRRaycastOrigin : IComponentData
    {
        /// <summary>
        /// The origin of the raycast for this component
        /// </summary>
        public ERayOrigin RayOrigin;
    }
}