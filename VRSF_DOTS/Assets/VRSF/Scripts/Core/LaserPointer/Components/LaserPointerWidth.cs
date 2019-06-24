using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.LaserPointer
{
    [RequireComponentTag(typeof(Raycast.VRRaycastOrigin))]
    public struct LaserPointerWidth : IComponentData
    {
        [HideInInspector] public float CurrentWidth;
        [HideInInspector] public float BaseWidth;
    }
}