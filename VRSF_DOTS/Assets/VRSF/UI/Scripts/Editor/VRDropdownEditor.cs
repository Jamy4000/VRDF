#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Dropdown
    /// </summary>
    [CustomEditor(typeof(VRDropdown), true)]
    [CanEditMultipleObjects]
    public class VRDropdownEditor : UnityEditor.UI.DropdownEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrDropdownPrefab;

        private VRDropdown vrDropdown;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            vrDropdown = (VRDropdown)target;
        }
        #endregion

        
        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrDropdown.gameObject, "Add BoxCollider");

            if (vrDropdown.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrDropdown.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrDropdown.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrDropdown.SetColliderAuto = false;
                vrDropdown.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrDropdown.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrDropdown.GetComponent<Collider>());
                    vrDropdown.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic DropDown Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Dropdown to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Dropdown", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Dropdown", priority = 0)]
        static void InstantiateVRDropDown(MenuCommand menuCommand)
        {
            vrDropdownPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRDropdown.prefab");

            // Create a custom game object
            GameObject newDropdown = PrefabUtility.InstantiatePrefab(vrDropdownPrefab) as GameObject;

            RectTransform rt = newDropdown.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newDropdown, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newDropdown, "Create " + newDropdown.name);
            Selection.activeObject = newDropdown;
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif