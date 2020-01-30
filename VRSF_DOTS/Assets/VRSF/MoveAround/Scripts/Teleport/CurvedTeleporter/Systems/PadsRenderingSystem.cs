using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class PadsRenderingSystem : ComponentSystem
    {
        private EntityQuery _parabolQuery;
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.EntityManager;
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
            var ppes = _parabolQuery.ToComponentDataArray<ParabolPadsEntities>(Allocator.TempJob);
            var parabolCalculations = _parabolQuery.ToComponentDataArray<ParabolCalculations>(Allocator.TempJob);

            for (int i = 0; i < gtps.Length; i++)
            {
                switch (gtps[i].CurrentTeleportState)
                {
                    case ETeleportState.Selecting:
                        // Render the Parabole's pads, aka the targets at the end of the parabole
                        RenderParabolePads(ctcs[i].PointToGoTo, ctcs[i].PointOnNavMesh || ctcs[i].PointOnTeleportableLayer, ppes[i], parabolCalculations[i].Normal);
                        break;
                    default:
                        if (_entityManager.GetEnabled(ppes[i].SelectionPadInstance))
                            _entityManager.SetEnabled(ppes[i].SelectionPadInstance, false);

                        if (_entityManager.GetEnabled(ppes[i].InvalidPadInstance))
                            _entityManager.SetEnabled(ppes[i].InvalidPadInstance, false);
                        break;
                }
            }

            gtps.Dispose();
            ctcs.Dispose();
            ppes.Dispose();
            parabolCalculations.Dispose();

            // TODO : Deactivate laser if it's still active
            //if (e.PointerObjects._ControllerPointer.enabled)
            //    ParabolicRendererHelper.ToggleHandLaser(e, false);
        }

        /// <summary>
        /// Render the targets of the parabola at the end of the curve, to give a visual feedback to the user on whether he can or cannot teleport.
        /// </summary>
        /// <param name="e">Entity to check</param>
        /// <param name="normal">The normal of the curve</param>
        private void RenderParabolePads(float3 pointToGoTo, bool isOnNavMesh, ParabolPadsEntities parabolObjects, Vector3 normal)
        {
            Entity currentPad;

            // Display the valid pad if the user is on the navMesh
            if (isOnNavMesh)
            {
                if (!_entityManager.GetEnabled(parabolObjects.SelectionPadInstance))
                    _entityManager.SetEnabled(parabolObjects.SelectionPadInstance, true);

                if (_entityManager.GetEnabled(parabolObjects.InvalidPadInstance))
                    _entityManager.SetEnabled(parabolObjects.InvalidPadInstance, false);

                currentPad = parabolObjects.SelectionPadInstance;
            }
            else
            {
                if (_entityManager.GetEnabled(parabolObjects.SelectionPadInstance))
                    _entityManager.SetEnabled(parabolObjects.SelectionPadInstance, false);

                if (!_entityManager.GetEnabled(parabolObjects.InvalidPadInstance))
                    _entityManager.SetEnabled(parabolObjects.InvalidPadInstance, true);

                currentPad = parabolObjects.InvalidPadInstance;
            }


            _entityManager.SetComponentData(currentPad, new Translation { Value = pointToGoTo + (new float3(1.0f, 1.0f, 1.0f) * 0.005f) });

            Quaternion newRotation = normal != Vector3.zero ? Quaternion.LookRotation(normal) : Quaternion.identity;
            newRotation.eulerAngles += new Vector3(90.0f, 0.0f, 0.0f);
            _entityManager.SetComponentData(currentPad, new Rotation { Value = newRotation });
        }
    }
}