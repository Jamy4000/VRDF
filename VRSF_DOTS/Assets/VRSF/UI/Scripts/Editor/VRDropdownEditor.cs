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
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private VRDropdown _dropdown;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _dropdown = (VRDropdown)target;
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
            Undo.RecordObject(_dropdown.gameObject, "Add BoxCollider");

            if (_dropdown.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _dropdown.SetColliderAuto = false;
                _dropdown.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_dropdown.GetComponent<Collider>());
                    _dropdown.gameObject.AddComponent<BoxCollider>();
                    _dropdown.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic DropDown Parameters", EditorStyles.boldLabel);

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
                PrefabUtility.RecordPrefabInstancePropertyModifications(_dropdown);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Dropdown to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Dropdown", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Dropdown", priority = 0)]
        static void InstantiateVRDropDown(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newDropdown = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRDropdown"));

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
    }
}
#endif