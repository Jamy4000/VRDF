using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public struct CurveTeleporterRendering : IComponentData
    {
        //public Mesh _parabolaMesh;

        /// <summary>
        /// Thickness of the parabola mesh
        /// </summary>
        public float GraphicThickness;

        /// <summary>
        /// Material to use to render the parabola mesh
        /// </summary>
        //public Material GraphicMaterial;
    }
}
