using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.LaserPointer
{
    [RequireComponentTag(typeof(Raycast.VRRaycastOrigin))]
    public struct LaserPointerWidth : IComponentData
    {
        [HideInInspector] public float BaseWidth;
        [HideInInspector] public float CurrentWidth;
    }
}