using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolPointsParameters : IComponentData
    {
        /// <summary>
        /// Number of points on the parabola mesh.  Greater point counts lead to a higher poly/smoother mesh.
        /// </summary>
        public int PointCount;

        /// <summary>
        /// Approximate spacing between each of the points on the parabola mesh.
        /// </summary>
        public float PointSpacing;
    }
}
