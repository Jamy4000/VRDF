#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Scrollbar
    /// </summary>
    [CustomEditor(typeof(VRScrollBar), true)]
    [CanEditMultipleObjects]
    public class VRScrollbarEditor : UnityEditor.UI.ScrollbarEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrScrollBarPrefab;

        private VRScrollBar vrScrollBar;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            vrScrollBar = (VRScrollBar)target;
        }
        #endregion
        

        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrScrollBar.gameObject, "Add BoxCollider");

            if (vrScrollBar.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrScrollBar.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrScrollBar.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrScrollBar.SetColliderAuto = false;
                vrScrollBar.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a vrScrollBar to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrScrollBar.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrScrollBar.GetComponent<Collider>());
                    vrScrollBar.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic ScrollBar Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Scrollbar to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Scrollbar/Vertical", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Scrollbar/Vertical", priority = 0)]
        static void InstantiateVRScrollbarVertical(MenuCommand menuCommand)
        {
            vrScrollBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRScrollbarVertical.prefab");

            // Create a custom game object
            GameObject newScrollbar = PrefabUtility.InstantiatePrefab(vrScrollBarPrefab) as GameObject;

            RectTransform rt = newScrollbar.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newScrollbar, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newScrollbar, "Create " + newScrollbar.name);
            Selection.activeObject = newScrollbar;
        }

        /// <summary>
        /// Add a new VR Scrollbar to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Scrollbar/Horizontal", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Scrollbar/Horizontal", priority = 0)]
        static void InstantiateVRScrollbarHorizontal(MenuCommand menuCommand)
        {
            vrScrollBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRScrollbarHorizontal.prefab");

            // Create a custom game object
            GameObject newScrollbar = PrefabUtility.InstantiatePrefab(vrScrollBarPrefab) as GameObject;

            RectTransform rt = newScrollbar.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newScrollbar, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newScrollbar, "Create " + newScrollbar.name);
            Selection.activeObject = newScrollbar;
        }
        #endregion

        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif