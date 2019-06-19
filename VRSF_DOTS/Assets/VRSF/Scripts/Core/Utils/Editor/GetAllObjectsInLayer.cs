#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VRSF.Utils.Editor
{
    /// <summary>
    /// Helper to select all objects in a specified layer.
    /// The objects in the Assets Folder, like prefabs, are as well selected.
    /// </summary>
    public class GetAllObjectsInLayer : EditorWindow
    {
        private int _layer = -1;

        [MenuItem("Window/VRSF/Util/Get Objects in layer")]
        public static void ShowWindow()
        {
            GetWindow<GetAllObjectsInLayer>("Get Objects in layer");
        }

        private void OnGUI()
        {
            // Add a Title
            GUILayout.Label("Get All the Objects in the specified layer", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _layer = EditorGUILayout.IntField("Layer to look for", _layer);

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Select objects in layer " + _layer) && _layer != -1)
            {
                SelectObjectsInLayer();
            }
        }

        private void SelectObjectsInLayer()
        {
            var objects = GetSceneObjects();
            GetObjectsInLayer(objects, _layer);
        }

        private static void GetObjectsInLayer(GameObject[] root, int layer)
        {
            List<GameObject> Selected = new List<GameObject>();
            foreach (GameObject t in root)
            {
                if (t.layer == layer)
                {
                    Selected.Add(t);
                }
            }
            Selection.objects = Selected.ToArray();

        }

        private static GameObject[] GetSceneObjects()
        {
            return Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(go => go.hideFlags == HideFlags.None).ToArray();
        }
    }
#endif
}