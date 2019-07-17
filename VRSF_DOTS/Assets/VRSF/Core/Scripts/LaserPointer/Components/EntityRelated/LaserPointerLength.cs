using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    [RequireComponentTag(typeof(VRRaycastOrigin))]
    public struct LaserPointerLength : IComponentData
    {
        [HideInInspector] public bool ShouldPointTo3DObjectsCenter;
        [HideInInspector] public bool ShouldPointToUICenter;
        [HideInInspector] public float BaseLength;
    }
}