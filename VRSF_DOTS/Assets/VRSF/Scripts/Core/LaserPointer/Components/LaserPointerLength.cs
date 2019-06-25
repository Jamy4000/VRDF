using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    [RequireComponentTag(typeof(VRRaycastOrigin))]
    public struct LaserPointerLength : IComponentData
    {
        [HideInInspector] public float BaseLength;
    }
}