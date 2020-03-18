using Unity.Mathematics;
using UnityEngine;

namespace VRDF.MoveAround.Fly
{
    public struct FlyDirection : Unity.Entities.IComponentData
    {
        /// <summary>
        /// Between 1 and -1
        /// </summary>
        public float FlightDirection;

        public Vector3 CurrentDirection;
    }
}