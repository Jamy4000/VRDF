using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public struct CurveTeleporterRendering : IComponentData
    {
        /// <summary>
        /// Thickness of the parabola mesh
        /// </summary>
        public float GraphicThickness;
    }
}
