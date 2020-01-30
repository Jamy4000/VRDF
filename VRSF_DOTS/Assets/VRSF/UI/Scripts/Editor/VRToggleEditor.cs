#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VRToggle
    /// </summary>
    [CustomEditor(typeof(VRToggle), true)]
    [CanEditMultipleObjects]
    public class VRToggleEditor : UnityEditor.UI.ToggleEditor
	{
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private SerializedProperty _clickableWithRaycast;
        private SerializedProperty _clickableUsingControllers;
        private SerializedProperty _onHoverEvent;
        private SerializedProperty _onStopHoverEvent;

        private VRToggle _toggle;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _toggle = (VRToggle)target;
            _setColliderAuto = serializedObject.FindProperty("SetColliderAuto");
            _clickableWithRaycast = serializedObject.FindProperty("LaserClickable");
            _clickableUsingControllers = serializedObject.FindProperty("ControllerClickable");
            _onHoverEvent = serializedObject.FindProperty("OnHover");
            _onStopHoverEvent = serializedObject.FindProperty("OnStopHovering");
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
            Undo.RecordObject(_toggle.gameObject, "Add BoxCollider");

            if (_toggle.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _toggle.SetColliderAuto = false;
                _toggle.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_toggle.GetComponent<Collider>());
                    _toggle.gameObject.AddComponent<BoxCollider>();
                    _toggle.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_toggle, "LaserClickable");
            EditorGUILayout.LabelField("Clickable using Laser Pointer with Click", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableWithRaycast);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_toggle, "MeshClickable");
            EditorGUILayout.LabelField("Clickable using Controllers' meshes", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableUsingControllers);
            CheckEndChanges();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Toggle Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_toggle, "OnHoverEvent");
            EditorGUILayout.PropertyField(_onHoverEvent);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_toggle, "OnStopHoverEvent");
            EditorGUILayout.PropertyField(_onStopHoverEvent);
            CheckEndChanges();
        }
        #endregion


        #region PRIVATE_METHODS
        private bool CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_toggle);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Toggle to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Toggle", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Toggle", priority = 0)]
        static void InstantiateVRToggle(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newToggle = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRToggle"));

            RectTransform rt = newToggle.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newToggle, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newToggle, "Create " + newToggle.name);
            Selection.activeObject = newToggle;
        }
        #endregion
    }
}
#endif