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
    public class VRInputFieldEditor : UnityEditor.UI.InputFieldEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        static GameObject vrInputFieldPrefab;

        private VRInputField vrInputField;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            vrInputField = (VRInputField)target;
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrInputField.gameObject, "Add BoxCollider");

            if (vrInputField.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrInputField.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrInputField.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrInputField.SetColliderAuto = false;
                vrInputField.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrInputField.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrInputField.GetComponent<Collider>());
                    vrInputField.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            vrInputField.LaserClickable = EditorGUILayout.ToggleLeft("Clickable using Raycast", vrInputField.LaserClickable);

            vrInputField.ControllerClickable = EditorGUILayout.ToggleLeft("Clickable using Controllers' meshes", vrInputField.ControllerClickable);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            vrInputField.UseVRKeyboard = EditorGUILayout.ToggleLeft("Use VRKeyboard", vrInputField.UseVRKeyboard);
            vrInputField.VRKeyboard = EditorGUILayout.ObjectField("VRKeyboard in scene", vrInputField.VRKeyboard, typeof(VRKeyboard), true) as VRKeyboard;

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic InputField Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
            
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR InputField to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Input Field", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR InputField", priority = 0)]
        static void InstantiateVRInputField(MenuCommand menuCommand)
        {
            vrInputFieldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRInputField.prefab");

            // Create a custom game object
            GameObject newInputField = PrefabUtility.InstantiatePrefab(vrInputFieldPrefab) as GameObject;

            RectTransform rt = newInputField.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newInputField, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newInputField, "Create " + newInputField.name);
            Selection.activeObject = newInputField;
        }
        #endregion PRIVATE_METHODS
    }
}
#endif 