using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolCalculations : IComponentData
    {
        public float3 Velocity;
        public float3 Normal;

        /// <summary>
        /// Reference to the pointer's distance
        /// </summary>
        public float _ControllerPointerDistance;
    }
}
