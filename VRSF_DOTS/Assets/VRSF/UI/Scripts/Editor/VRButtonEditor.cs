#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Core.Utils;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VRButton
    /// </summary>
    [CustomEditor(typeof(VRButton), true)]
    [CanEditMultipleObjects]
    public class VRButtonEditor : UnityEditor.UI.ButtonEditor
	{
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private SerializedProperty _clickableWithRaycast;
        private SerializedProperty _clickableUsingControllers;
        private SerializedProperty _onHoverEvent;
        private SerializedProperty _onStopHoverEvent;

        private VRButton _button;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _button = (VRButton)target;
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
            Undo.RecordObject(_button.gameObject, "Add BoxCollider");

            if (_button.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _button.SetColliderAuto = false;
                _button.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);
                
                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_button.GetComponent<Collider>());
                    _button.gameObject.AddComponent<BoxCollider>();
                    _button.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_button, "LaserClickable");
            EditorGUILayout.LabelField("Clickable using Laser Pointer with Click", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableWithRaycast);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_button, "MeshClickable");
            EditorGUILayout.LabelField("Clickable using Controllers' meshes", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableUsingControllers);
            CheckEndChanges();

            EditorGUILayout.Space();

            // Add a button to call the OnClick Event if the application is playing
            if (Application.isPlaying && GUILayout.Button("Invoke OnClick Event"))
                _button.onClick.Invoke();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Button Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_button, "OnHoverEvent");
            EditorGUILayout.PropertyField(_onHoverEvent);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_button, "OnStopHoverEvent");
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
                PrefabUtility.RecordPrefabInstancePropertyModifications(_button);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Button to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Button", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Button", priority = 0)]
        static void InstantiateVRButton(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newButton = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("VRButton"));

            RectTransform rt = newButton.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newButton, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newButton, "Create " + newButton.name);
            Selection.activeObject = newButton;
        }
        #endregion
    }
}
#endif