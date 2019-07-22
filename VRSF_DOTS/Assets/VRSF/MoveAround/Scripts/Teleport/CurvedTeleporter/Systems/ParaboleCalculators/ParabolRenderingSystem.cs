using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolRenderingSystem : ComponentSystem
    {
        private EntityQuery _pointsQuery;
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.Active.EntityManager;
            _pointsQuery = GetEntityQuery(typeof(Translation), typeof(ParabolPointParent));
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref GeneralTeleportParameters gtp, ref CurveTeleporterCalculations ctc, ref CurveTeleporterRendering ctr, ref ParabolPointsParameters ppp, ref ParabolPadPrefabs parabolObjects, ref ParabolCalculations parabolCalculations) =>
            {
                switch (gtp.CurrentTeleportState)
                {
                    case ETeleportState.Selecting:
                        var parabolMesh = _entityManager.GetSharedComponentData<RenderMesh>(e);

                        _pointsQuery.SetFilter(new ParabolPointParent { TeleporterEntityIndex = e.Index });
                        var points = _pointsQuery.ToComponentDataArray<Translation>(Unity.Collections.Allocator.TempJob);

                        // Render the Parabole's pads, aka the targets at the end of the parabole
                        ParabolicRendererHelper.RenderParabolePads(ctc.PointToGoTo, ctc.PointOnNavMesh, parabolObjects, parabolCalculations.Normal);

                        // Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
                        ParabolicRendererHelper.GenerateMesh(ref parabolMesh.mesh, points, ctc.LastPointIndex, parabolCalculations.Velocity, Time.time % 1, ctr.GraphicThickness);
                        Graphics.DrawMesh(parabolMesh.mesh, Matrix4x4.identity, parabolMesh.material, parabolMesh.layer);
                        points.Dispose();
                        break;
                }
            });
            // TODO : Deactivate laser if it's still active
            //if (e.PointerObjects._ControllerPointer.enabled)
            //    ParabolicRendererHelper.ToggleHandLaser(e, false);

            //

        }
    }
}