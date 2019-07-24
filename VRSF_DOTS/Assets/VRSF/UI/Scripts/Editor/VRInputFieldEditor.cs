#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Add the public variable of VR input Field to the Editor and add the UseVRKeyboard and VRKeyboard reference to the Inspector
    /// </summary>
    [CustomEditor(typeof(VRInputField), true)]
    [CanEditMultipleObjects]
    public class VRInputFieldEditor : TMPro.EditorUtilities.TMP_InputFieldEditor
    {
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private SerializedProperty _clickableWithRaycast;
        private SerializedProperty _clickableUsingControllers;
        private SerializedProperty _useVRKeyboard;
        private SerializedProperty _vrKeyboardInScene;

        private VRInputField _inputField;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _inputField = (VRInputField)target;
            _setColliderAuto = serializedObject.FindProperty("SetColliderAuto");
            _clickableWithRaycast = serializedObject.FindProperty("LaserClickable");
            _clickableUsingControllers = serializedObject.FindProperty("ControllerClickable");
            _useVRKeyboard = serializedObject.FindProperty("UseVRKeyboard");
            _vrKeyboardInScene = serializedObject.FindProperty("VRKeyboard");
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
            Undo.RecordObject(_inputField.gameObject, "Add BoxCollider");

            if (_inputField.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _inputField.SetColliderAuto = false;
                _inputField.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_inputField.GetComponent<Collider>());
                    _inputField.gameObject.AddComponent<BoxCollider>();
                    _inputField.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_inputField, "LaserClickable");
            EditorGUILayout.LabelField("Clickable using Laser Pointer with Click", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableWithRaycast);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_inputField, "MeshClickable");
            EditorGUILayout.LabelField("Clickable using Controllers' meshes", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableUsingControllers);
            CheckEndChanges();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_inputField, "useVRKeyboard");
            EditorGUILayout.PropertyField(_useVRKeyboard);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_inputField, "VRKeyboard");
            EditorGUILayout.PropertyField(_vrKeyboardInScene);
            CheckEndChanges();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic InputField Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private bool CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_inputField);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR InputField to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/Input Field/VR Input Field", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/Input Field/VR Input Field", priority = 0)]
        static void InstantiateVRInputField(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newInputField = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRInputField"));

            RectTransform rt = newInputField.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newInputField, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newInputField, "Create " + newInputField.name);
            Selection.activeObject = newInputField;
        }

        /// <summary>
        /// Add a new VR Keyboard to the Scene
        /// </summary>
        [MenuItem("VRSF/UI/Input Field/VR Keyboard", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/Input Field/VR Keyboard", priority = 0)]
        static void InstantiateVRKeyboard(MenuCommand _)
        {
            // Create a custom game object
            GameObject newKeyboard = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRKeyboard"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newKeyboard, null);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newKeyboard, "Create " + newKeyboard.name);
            Selection.activeObject = newKeyboard;
        }
        #endregion PRIVATE_METHODS
    }
}
#endif 