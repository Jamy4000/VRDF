#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VRButton
    /// </summary>
    [CustomEditor(typeof(VRButton), true)]
    [CanEditMultipleObjects]
    public class VRButtonEditor : UnityEditor.UI.ButtonEditor
	{
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrButtonPrefab;
        private VRButton button;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            button = (VRButton)target;
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(button.gameObject, "Add BoxCollider");

            if (button.gameObject.GetComponent<BoxCollider>() != null)
            {
                button.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", button.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                button.SetColliderAuto = false;
                button.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    button.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(button.GetComponent<Collider>());
                    button.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            button.LaserClickable = EditorGUILayout.ToggleLeft("Clickable using Raycast", button.LaserClickable);
            
            button.ControllerClickable = EditorGUILayout.ToggleLeft("Clickable using Controllers' meshes", button.ControllerClickable);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Add a button to call the OnClick Event if the application is playing
            if (Application.isPlaying && GUILayout.Button("Invoke OnClick Event"))
            {
                button.onClick.Invoke();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Button Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
            
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Button to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Button", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Button", priority = 0)]
        static void InstantiateVRButton(MenuCommand menuCommand)
        {
            vrButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRButton.prefab");
            
            // Create a custom game object
            GameObject newButton = PrefabUtility.InstantiatePrefab(vrButtonPrefab) as GameObject;
            
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


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif