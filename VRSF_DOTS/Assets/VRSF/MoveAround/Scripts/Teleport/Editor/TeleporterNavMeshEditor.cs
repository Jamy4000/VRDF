#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Custom inspector for ViveNavMesh.  This handles the conversion from Unity NavMesh to Mesh and performs some
    /// computational geometry to find the borders of the mesh.
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [CustomEditor(typeof(TeleportNavMeshAuthoring))]
    public class TeleporterNavMeshEditor : Editor
    {
        #region PRIVATE_VARIABLES
        private SerializedProperty p_area;
        private SerializedProperty p_mesh;
        private SerializedProperty p_query_trigger_interaction;
        private SerializedProperty p_sample_radius;
        private SerializedProperty p_sphereCast_radius; 
        private SerializedProperty p_ignore_sloped_surfaces;
        private SerializedProperty p_dewarp_method;

        private static List<int> layerNumbers = new List<int>();
        #endregion PRIVATE_VARIABLES


        #region EDITOR_METHODS
        private void OnEnable()
        {
            p_area = serializedObject.FindProperty("NavAreaMask");
            p_mesh = serializedObject.FindProperty("SelectableMesh");
            p_query_trigger_interaction = serializedObject.FindProperty("QueryTriggerInteraction");
            p_sample_radius = serializedObject.FindProperty("SampleRadius");
            p_sphereCast_radius = serializedObject.FindProperty("SphereCastRadius");
            p_ignore_sloped_surfaces = serializedObject.FindProperty("IgnoreSlopedSurfaces");
            p_dewarp_method = serializedObject.FindProperty("DewarpingMethod");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle bold_wrap = EditorStyles.boldLabel;
            bold_wrap.wordWrap = true;
            GUILayout.Label("Navmesh Preprocessor for VR Locomotion", bold_wrap);
            GUILayout.Label("Based on Adrian Biagioli work, 2017", EditorStyles.miniLabel);
            GUILayout.Label("Updated by Arnaud Briche, 2018", EditorStyles.miniLabel);

            EditorGUILayout.Space();

            GUILayout.Label("Before Using", bold_wrap);
            GUIStyle wrap = EditorStyles.label;
            wrap.wordWrap = true;
            GUILayout.Label(
                "Make sure you bake a Navigation Mesh (NavMesh) in Unity before continuing (Window > AI > Navigation).  When you " +
                "are done, click \"Update Navmesh Data\" below.  This will update the graphic of the playable area " +
                "that the player will see in-game.\n",
                wrap);

            TeleportNavMeshAuthoring tnmAuthoring = (TeleportNavMeshAuthoring)target;

            serializedObject.Update();

            // Area Mask //
            string[] areaNames = GameObjectUtility.GetNavMeshAreaNames();
            int[] area_index = new int[areaNames.Length];
            int temp_mask = 0;
            for (int x = 0; x < areaNames.Length; x++)
            {
                area_index[x] = GameObjectUtility.GetNavMeshAreaFromName(areaNames[x]);
                temp_mask |= ((p_area.intValue >> area_index[x]) & 1) << x;
            }
            EditorGUI.BeginChangeCheck();
            temp_mask = EditorGUILayout.MaskField("Area Mask", temp_mask, areaNames);
            if (EditorGUI.EndChangeCheck())
            {
                p_area.intValue = 0;
                for (int x = 0; x < areaNames.Length; x++)
                    p_area.intValue |= (((temp_mask >> x) & 1) == 1 ? 0 : 1) << area_index[x];
                p_area.intValue = ~p_area.intValue;
            }
            serializedObject.ApplyModifiedProperties();

            // Sanity check for Null properties //
            bool HasMesh = tnmAuthoring.SelectableMesh != null && tnmAuthoring.SelectableMesh.vertexCount != 0;

            // Fixes below error message popping up with prefabs.  Kind of hacky but gets the job done
            bool isPrefab = EditorUtility.IsPersistent(target);
            if (isPrefab && tnmAuthoring.SelectableMesh == null)
                tnmAuthoring.SelectableMesh = new Mesh();

            bool MeshNull = tnmAuthoring.SelectableMesh == null;

            if (MeshNull)
            {
                string str = "Internal Error: Selectable Mesh == null. ";
                str += "This may lead to strange behavior or serialization.  Try updating the mesh or delete and recreate the Navmesh object.  ";
                str += "If you are able to consistently get a Vive Nav Mesh object into this state, please submit a bug report.";
                EditorGUILayout.HelpBox(str, MessageType.Error);
            }

            // Update / Clear Navmesh Data //
            if (GUILayout.Button("Update Navmesh Data"))
            {
                Undo.RecordObject(tnmAuthoring, "Update Navmesh Data");

                NavMeshTriangulation tri = NavMesh.CalculateTriangulation();

                Vector3[] verts = tri.vertices;
                int[] tris = tri.indices;
                int[] areas = tri.areas;

                int vert_size = verts.Length;
                int tri_size = tris.Length;
                //RemoveMeshDuplicates(verts, tris, out vert_size, 0.01f);
                DewarpMesh(verts, tnmAuthoring.DewarpingMethod, tnmAuthoring.SampleRadius);
                CullNavmeshTriangulation(verts, tris, areas, p_area.intValue, tnmAuthoring.IgnoreSlopedSurfaces, ref vert_size, ref tri_size);

                Mesh m = ConvertNavmeshToMesh(verts, tris, vert_size, tri_size);

                serializedObject.Update();
                p_mesh.objectReferenceValue = m;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            GUI.enabled = HasMesh;
            if (GUILayout.Button("Clear Navmesh Data"))
            {
                Undo.RecordObject(tnmAuthoring, "Clear Navmesh Data");

                // Note: Unity does not serialize "null" correctly so we set everything to empty objects
                Mesh m = new Mesh();

                serializedObject.Update();
                p_mesh.objectReferenceValue = m;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            GUI.enabled = true;

            GUILayout.Label(HasMesh ? "Status: NavMesh Loaded" : "Status: No NavMesh Loaded");

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            // Raycast Settings //
            EditorGUILayout.LabelField("Raycast Settings", EditorStyles.boldLabel);
            
            QueryTriggerInteraction temp_query_trigger_interaction = (QueryTriggerInteraction)p_query_trigger_interaction.intValue;

            EditorGUI.BeginChangeCheck();
            temp_query_trigger_interaction = (QueryTriggerInteraction)EditorGUILayout.EnumPopup("Query Trigger Interaction", (QueryTriggerInteraction)temp_query_trigger_interaction);
            if (EditorGUI.EndChangeCheck())
            {
                p_query_trigger_interaction.intValue = (int)temp_query_trigger_interaction;
            }
            serializedObject.ApplyModifiedProperties();
        
            EditorGUILayout.Space();

            // Navmesh Settings //
            EditorGUILayout.LabelField("Navmesh Settings", EditorStyles.boldLabel);
            GUILayout.Label(
                "Make sure the sample radius below is equal to your Navmesh Voxel Size (see Advanced > Voxel Size " +
                "in the navigation window).  Increase this if the selection disk is not appearing.",
                wrap);
            EditorGUILayout.PropertyField(p_sample_radius);
            EditorGUILayout.PropertyField(p_sphereCast_radius);
            EditorGUILayout.PropertyField(p_ignore_sloped_surfaces);
            EditorGUILayout.PropertyField(p_dewarp_method);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion EDITOR_METHODS


        #region PRIVATE_METHODS
        private static void DewarpMesh(Vector3[] verts, ENavmeshDewarpingMethod dw, float step)
        {
            if (dw == ENavmeshDewarpingMethod.None)
                return;

            for (int x = 0; x < verts.Length; x++)
            {
                if (dw == ENavmeshDewarpingMethod.RaycastDownward)
                {

                    // Have the raycast span over the entire navmesh voxel
                    Vector3 sample = verts[x];
                    double vy = Math.Round(verts[x].y / step) * step;
                    sample.y = (float)vy;

                    if (Physics.Raycast(sample, Vector3.down, out RaycastHit hit, (float)step + 0.01f))
                        verts[x] = hit.point;

                }
                else if (dw == ENavmeshDewarpingMethod.RoundToVoxelSize)
                {
                    // Clamp the point to the voxel grid in the Y direction
                    double vy = Math.Round((verts[x].y - 0.05) / step) * step + 0.05;
                    verts[x].y = (float)vy;
                }
            }
        }

        /// <summary>
        /// Modifies the given NavMesh so that only the Navigation areas are present in the mesh.  This is done only 
        /// by swapping, so that no new memory is allocated.
        /// 
        /// The data stored outside of the returned array sizes should be considered invalid and will contain garbage data.
        /// </summary>
        /// 
        /// <param name="vertices">vertices of Navmesh</param>
        /// <param name="indices">indices of Navmesh triangles</param>
        /// <param name="areas">Navmesh areas</param>
        /// <param name="areaMask">Area mask to include in returned mesh.  Areas outside of this mask are culled.</param>
        /// <param name="ignore_sloped_surfaces">Zheter we ignore the sloped surface or not</param>
        /// <param name="vert_size">New size of navMesh.vertices</param>
        /// <param name="tri_size">New size of navMesh.areas and one third of the size of navMesh.indices</param>
        private static void CullNavmeshTriangulation(Vector3[] vertices, int[] indices, int[] areas, int areaMask, bool ignore_sloped_surfaces, ref int vert_size, ref int tri_size)
        {
            // Step 1: re-order triangles so that valid areas are in front.  Then determine tri_size.
            tri_size = indices.Length / 3;
            for (int i = 0; i < tri_size; i++)
            {
                Vector3 p1 = vertices[indices[i * 3]];
                Vector3 p2 = vertices[indices[i * 3 + 1]];
                Vector3 p3 = vertices[indices[i * 3 + 2]];
                Plane p = new Plane(p1, p2, p3);
                bool vertical = Mathf.Abs(Vector3.Dot(p.normal, Vector3.up)) > 0.99f;

                // If the current triangle isn't flat (normal is up) or if it doesn't match
                // with the provided mask, we should cull it.
                if (((1 << areas[i]) & areaMask) == 0 || (ignore_sloped_surfaces && !vertical)) // If true this triangle should be culled.
                {
                    // Swap area indices and triangle indices with the end of the array
                    int t_ind = tri_size - 1;

                    int t_area = areas[t_ind];
                    areas[t_ind] = areas[i];
                    areas[i] = t_area;

                    for (int j = 0; j < 3; j++)
                    {
                        int t_v = indices[t_ind * 3 + j];
                        indices[t_ind * 3 + j] = indices[i * 3 + j];
                        indices[i * 3 + j] = t_v;
                    }

                    // Then reduce the size of the array, effectively cutting off the previous triangle
                    tri_size--;
                    // Stay on the same index so that we can check the triangle we just swapped.
                    i--;
                }
            }

            // Step 2: Cull the vertices that aren't used.
            vert_size = 0;
            for (int i = 0; i < tri_size * 3; i++)
            {
                int prv = indices[i];
                if (prv >= vert_size)
                {
                    int nxt = vert_size;

                    // Bring the current vertex to the end of the "active" array by swapping it with what's currently there
                    Vector3 t_v = vertices[prv];
                    vertices[prv] = vertices[nxt];
                    vertices[nxt] = t_v;

                    // Now change around the values in the triangle indices to reflect the swap
                    for (int j = i; j < tri_size * 3; j++)
                    {
                        if (indices[j] == prv)
                            indices[j] = nxt;
                        else if (indices[j] == nxt)
                            indices[j] = prv;
                    }

                    // Increase the size of the vertex array to reflect the changes.
                    vert_size++;
                }
            }
        }

        /// <summary>
        /// Converts a NavMesh (or a NavMesh area) into a standard Unity mesh.  This is later used
        /// to render the mesh on-screen using Unity's standard rendering tools.
        /// </summary>
        /// 
        /// <param name="vraw"></param>
        /// <param name="iraw"></param>
        /// <param name="vert_size">size of vertex array</param>
        /// <param name="tri_size">size of triangle array</param>
        /// 
        /// <returns>The new Mesh created from the NavMesh</returns>
        private static Mesh ConvertNavmeshToMesh(Vector3[] vraw, int[] iraw, int vert_size, int tri_size)
        {
            Mesh ret = new Mesh();

            if (vert_size >= 65535)
            {
                Debug.LogError("Playable NavMesh too big (vertex count >= 65535)!  Limit the size of the playable area using" +
                    "Area Masks.  For now no preview mesh will render.");
                return ret;
            }

            Vector3[] vertices = new Vector3[vert_size];
            for (int x = 0; x < vertices.Length; x++)
            {
                vertices[x].x = vraw[x].x;
                vertices[x].y = vraw[x].y;
                vertices[x].z = vraw[x].z;
            }

            int[] triangles = new int[tri_size * 3];
            for (int x = 0; x < triangles.Length; x++)
                triangles[x] = iraw[x];

            ret.name = "Navmesh";

            ret.Clear();
            ret.vertices = vertices;
            ret.triangles = triangles;

            ret.RecalculateNormals();
            ret.RecalculateBounds();

            return ret;
        }

        /// <summary>
        /// VERY naive implementation of removing duplicate vertices in a mesh.  O(n^2).
        /// 
        /// This is necessary because Unity NavMeshes for some reason have a whole bunch of duplicate vertices (or vertices
        /// that are very close together).  So some processing needs to be done go get rid of these.
        /// 
        /// If this becomes an actual performance hog, consider changing this to sort the vertices first using a more
        /// optimized process O(n lg n) then removing adjacent duplicates.
        /// </summary>
        /// 
        /// <param name="verts">Vertex array to process</param>
        /// <param name="elts">Triangle indices array</param>
        /// <param name="verts_size">size of vertex array after processing</param>
        /// <param name="threshold">Threshold with which to combine vertices</param>
        private static void RemoveMeshDuplicates(Vector3[] verts, int[] elts, out int verts_size, double threshold)
        {
            int size = verts.Length;
            for (int x = 0; x < size; x++)
            {
                for (int y = x + 1; y < size; y++)
                {
                    Vector3 d = verts[x] - verts[y];

                    if (x != y && Mathf.Abs(d.x) < threshold && Mathf.Abs(d.y) < threshold && Mathf.Abs(d.z) < threshold)
                    {
                        verts[y] = verts[size - 1];
                        for (int z = 0; z < elts.Length; z++)
                        {
                            if (elts[z] == y)
                                elts[z] = x;

                            if (elts[z] == size - 1)
                                elts[z] = y;
                        }
                        size--;
                        y--;
                    }
                }
            }

            verts_size = size;
        }

        /// <summary>
        /// Given a list of edges, finds a polyline connected to the edge at index start.
        /// Guaranteed to run in O(n) time.  Assumes that each edge only has two neighbor edges.
        /// </summary>
        /// <param name="start">starting index of edge</param>
        /// <param name="visited">tally of visited edges (perhaps from previous calls)</param>
        /// <param name="edges">list of edges</param>
        /// <returns></returns>
        private static int[] FindPolylineFromEdges(int start, bool[] visited, List<Edge> edges)
        {
            List<int> loop = new List<int>(edges.Count);
            loop.Add(edges[start].Min);
            loop.Add(edges[start].Max);
            visited[start] = true;

            // With each iteration of this while loop, we look for an edge that connects to the previous one
            // but hasn't been processed yet (to prevent simply finding the previous edge again, and to prevent
            // a hang if faulty data is given).
            while (loop[loop.Count - 1] != edges[start].Min)
            {
                int cur = loop[loop.Count - 1];
                bool found = false;
                for (int x = 0; x < visited.Length; x++)
                {
                    // If we have visited this edge before, or both vertices on this edge
                    // aren't connected to cur, then skip the edge
                    if (!visited[x] && (edges[x].Min == cur || edges[x].Max == cur))
                    {
                        // The next vertex in the loop
                        int next = edges[x].Min == cur ? edges[x].Max : edges[x].Min;
                        loop.Add(next);

                        // Mark the edge as visited and continue the outermost loop
                        visited[x] = true;
                        found = true;
                        break;
                    }
                }
                if (!found)// acyclic, so break.
                    break;
            }

            int[] ret = new int[loop.Count];
            loop.CopyTo(ret);
            return ret;
        }

        // From http://answers.unity3d.com/questions/42996/how-to-create-layermask-field-in-a-custom-editorwi.html
        static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            var layers = UnityEditorInternal.InternalEditorUtility.layers;

            layerNumbers.Clear();

            for (int i = 0; i < layers.Length; i++)
                layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }

            maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;

            return layerMask;
        }
        #endregion PRIVATE_METHODS
    }
#endif
}