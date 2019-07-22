using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public static class ParabolicRendererHelper
    {
        /// <summary>
        /// Render the targets of the parabola at the end of the curve, to give a visual feedback to the user on whether he can or cannot teleport.
        /// </summary>
        /// <param name="e">Entity to check</param>
        /// <param name="normal">The normal of the curve</param>
        public static void RenderParabolePads(float3 tempPointToGoTo, bool isOnNavMesh, ParabolPadPrefabs parabolObjects, Vector3 normal)
        {
            // TODO
            //// Display the valid pad if the user is on the navMesh
            //if (e.PointerObjects._selectionPadObject != null)
            //{
            //    e.PointerObjects._selectionPadObject.SetActive(isOnNavMesh);
            //    if (e.PointerCalculations.PointOnNavMesh)
            //    {
            //        e.PointerObjects._selectionPadObject.transform.position = tempPointToGoTo + (new float3(1.0f, 1.0f, 1.0f) * 0.005f);
            //        e.PointerObjects._selectionPadObject.transform.rotation = Quaternion.LookRotation(normal);
            //        e.PointerObjects._selectionPadObject.transform.Rotate(90, 0, 0);
            //    }
            //}

            //// Display the invalid pad if the user is not on the navMesh
            //if (e.PointerObjects._invalidPadObject != null)
            //{
            //    e.PointerObjects._invalidPadObject.SetActive(!isOnNavMesh);
            //    if (!e.PointerCalculations.PointOnNavMesh)
            //    {
            //        e.PointerObjects._invalidPadObject.transform.position = tempPointToGoTo + (new float3(1.0f, 1.0f, 1.0f) * 0.005f);
            //        e.PointerObjects._invalidPadObject.transform.rotation = Quaternion.LookRotation(normal);
            //        e.PointerObjects._invalidPadObject.transform.Rotate(90, 0, 0);
            //    }
            //}
        }

        /// <summary>
        /// Generate the mesh of the parabole
        /// </summary>
        /// <param name="m">The mesh to generate, pass as reference</param>
        /// <param name="points">The list of points on the path of the parabole</param>
        /// <param name="fwd">The forward Vector for the parabole</param>
        /// <param name="uvoffset">The offset for the UV</param>
        /// <param name="graphicThickness">The thickness of the parabole to dispaly</param>
        public static void GenerateMesh(ref Mesh m, NativeArray<Translation> points, int lastPointIndex, Vector3 fwd, float uvoffset, float graphicThickness)
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


        /// <summary>
        /// Activate/Deactivate the pointer on the left hand
        /// </summary>
        /// <param name="active"></param>
        public static void ToggleHandLaser()//ParabolicPointerUpdateSystem.Filter e, bool active)
        {
            // TODO: Pass all LaserPointerState to FORCE_OFF

            //// We deactivate the fact that the user is able to click on stuffs as long as the curve teleport is on
            //if (e.BAC_Comp.ButtonHand == EHand.LEFT)
            //    PointerClickComponent.LeftTriggerCanClick = active;
            //else
            //    PointerClickComponent.RightTriggerCanClick = active;

            //if (e.PointerObjects._ControllerPointer != null)
            //{
            //    // We change the status of the laser gameObject
            //    e.PointerObjects._ControllerPointer.enabled = active;
            //}
        }
    }
}