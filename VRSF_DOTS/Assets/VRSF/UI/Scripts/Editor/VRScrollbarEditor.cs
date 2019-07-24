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
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private VRScrollBar _scrollbar;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _scrollbar = (VRScrollBar)target;
            _setColliderAuto = serializedObject.FindProperty("SetColliderAuto");
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_scrollbar.gameObject, "Add BoxCollider");

            if (_scrollbar.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _scrollbar.SetColliderAuto = false;
                _scrollbar.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_scrollbar.GetComponent<Collider>());
                    _scrollbar.gameObject.AddComponent<BoxCollider>();
                    _scrollbar.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic ScrollBar Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
        #endregion


        #region PRIVATE_METHODS
        private bool CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_scrollbar);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Scrollbar to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Scrollbar/Vertical", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Scrollbar/Vertical", priority = 0)]
        static void InstantiateVRScrollbarVertical(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newScrollbar = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRScrollbarVertical"));

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
            // Create a custom game object
            GameObject newScrollbar = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRScrollbarHorizontal"));

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
    }
}
#endif