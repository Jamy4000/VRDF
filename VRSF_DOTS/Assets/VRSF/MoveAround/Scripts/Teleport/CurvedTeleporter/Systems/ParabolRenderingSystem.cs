using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
        private EntityQuery _parabolQuery;
        private EntityQuery _pointsQuery;
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.Active.EntityManager;
            _pointsQuery = GetEntityQuery(typeof(Translation), typeof(ParabolPointParent));
            _parabolQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GeneralTeleportParameters),
                    typeof(CurveTeleporterCalculations),
                    typeof(CurveTeleporterRendering),
                    typeof(ParabolPointsParameters),
                    typeof(ParabolPadsEntities),
                    typeof(ParabolCalculations)
                }
            });
        }

        protected override void OnUpdate()
        {
            var gtps = _parabolQuery.ToComponentDataArray<GeneralTeleportParameters>(Allocator.TempJob);
            var ctcs = _parabolQuery.ToComponentDataArray<CurveTeleporterCalculations>(Allocator.TempJob);
            var ctrs = _parabolQuery.ToComponentDataArray<CurveTeleporterRendering>(Allocator.TempJob);
            var parabolCalculations = _parabolQuery.ToComponentDataArray<ParabolCalculations>(Allocator.TempJob);
            var entities = _parabolQuery.ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < gtps.Length; i++)
            {
                if (gtps[i].CurrentTeleportState == ETeleportState.Selecting)
                {
                    var parabolMesh = _entityManager.GetSharedComponentData<RenderMesh>(entities[i]);

                    _pointsQuery.SetFilter(new ParabolPointParent { TeleporterEntityIndex = entities[i].Index });
                    var points = _pointsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

                    // Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
                    ParabolicRendererHelper.GenerateMesh(ref parabolMesh.mesh, points, ctcs[i].LastPointIndex, parabolCalculations[i].Velocity, Time.time % 1, ctrs[i].GraphicThickness);
                    points.Dispose();

                    Graphics.DrawMesh(parabolMesh.mesh, Matrix4x4.identity, parabolMesh.material, parabolMesh.layer);
                }
            }

            gtps.Dispose();
            ctcs.Dispose();
            ctrs.Dispose();
            parabolCalculations.Dispose();
            entities.Dispose();

            // TODO : Deactivate laser if it's still active
            //if (e.PointerObjects._ControllerPointer.enabled)
            //    ParabolicRendererHelper.ToggleHandLaser(e, false);
        }
    }
}