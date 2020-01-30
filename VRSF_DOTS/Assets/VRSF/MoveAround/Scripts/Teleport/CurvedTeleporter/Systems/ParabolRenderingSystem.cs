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
        private EntityQuery _pointsQuery;
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.EntityManager;
            _pointsQuery = GetEntityQuery(typeof(Translation), typeof(ParabolPointParent));
        }

        [Unity.Burst.BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref GeneralTeleportParameters gtp, ref CurveTeleporterCalculations ctc, ref CurveTeleporterRendering ctr, ref ParabolCalculations parabolCalculations) =>
            {
                if (gtp.CurrentTeleportState == ETeleportState.Selecting)
                {
                    var parabolMesh = _entityManager.GetSharedComponentData<RenderMesh>(e);

                    _pointsQuery.SetSharedComponentFilter(new ParabolPointParent { TeleporterEntityIndex = e.Index });
                    var points = _pointsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

                    // Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
                    GenerateMesh(ref parabolMesh.mesh, points, ctc.LastPointIndex, parabolCalculations.Velocity, (float)Time.ElapsedTime % 1, ctr.GraphicThickness);
                    points.Dispose();

                    Graphics.DrawMesh(parabolMesh.mesh, Matrix4x4.identity, parabolMesh.material, parabolMesh.layer);
                }
            });
            // TODO : Deactivate laser if it's still active
            //if (e.PointerObjects._ControllerPointer.enabled)
            //    ParabolicRendererHelper.ToggleHandLaser(e, false);
        }

        /// <summary>
        /// Generate the mesh of the parabole
        /// </summary>
        /// <param name="m">The mesh to generate, pass as reference</param>
        /// <param name="points">The list of points on the path of the parabole</param>
        /// <param name="fwd">The forward Vector for the parabole</param>
        /// <param name="uvoffset">The offset for the UV</param>
        /// <param name="graphicThickness">The thickness of the parabole to dispaly</param>
        private void GenerateMesh(ref Mesh m, NativeArray<Translation> points, int lastPointIndex, Vector3 fwd, float uvoffset, float graphicThickness)
        {
            try
            {
                Vector3[] verts = new Vector3[lastPointIndex * 2];
                Vector2[] uv = new Vector2[lastPointIndex * 2];

                float3 right = Vector3.Cross(fwd, Vector3.up).normalized;

                for (int x = 0; x < lastPointIndex; x++)
                {
                    verts[2 * x] = points[x].Value - right * graphicThickness / 2;
                    verts[2 * x + 1] = points[x].Value + right * graphicThickness / 2;

                    float uvoffset_mod = uvoffset;
                    if (x == lastPointIndex - 1 && x > 1)
                    {
                        float dist_last = ((Vector3)points[x - 2].Value - (Vector3)points[x - 1].Value).magnitude;
                        float dist_cur = ((Vector3)points[x].Value - (Vector3)points[x - 1].Value).magnitude;
                        uvoffset_mod += 1 - dist_cur / dist_last;
                    }

                    uv[2 * x] = new Vector2(0, x - uvoffset_mod);
                    uv[2 * x + 1] = new Vector2(1, x - uvoffset_mod);
                }

                int[] indices = new int[2 * 3 * (verts.Length - 2)];
                for (int x = 0; x < verts.Length / 2 - 1; x++)
                {
                    int p1 = 2 * x;
                    int p2 = 2 * x + 1;
                    int p3 = 2 * x + 2;
                    int p4 = 2 * x + 3;

                    indices[12 * x] = p1;
                    indices[12 * x + 1] = p2;
                    indices[12 * x + 2] = p3;
                    indices[12 * x + 3] = p3;
                    indices[12 * x + 4] = p2;
                    indices[12 * x + 5] = p4;

                    indices[12 * x + 6] = p3;
                    indices[12 * x + 7] = p2;
                    indices[12 * x + 8] = p1;
                    indices[12 * x + 9] = p4;
                    indices[12 * x + 10] = p2;
                    indices[12 * x + 11] = p3;
                }

                m.Clear();
                m.vertices = verts;
                m.uv = uv;
                m.triangles = indices;
                m.RecalculateBounds();
                m.RecalculateNormals();
            }
            catch (System.Exception e)
            {
                Debug.LogError("<b>[VRSF] :</b> An error has occured while rendering the Curve Parabole :\n" + e.ToString());
            }
        }
    }
}