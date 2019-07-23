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
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrTogglePrefab;

        private VRToggle vrToggle;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            vrToggle = (VRToggle)target;
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrToggle.gameObject, "Add BoxCollider");

            if (vrToggle.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrToggle.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrToggle.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrToggle.SetColliderAuto = false;
                vrToggle.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrToggle.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrToggle.GetComponent<Collider>());
                    vrToggle.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            vrToggle.LaserClickable = EditorGUILayout.ToggleLeft("Clickable using Raycast", vrToggle.LaserClickable);
            vrToggle.ControllerClickable = EditorGUILayout.ToggleLeft("Clickable using Controllers' meshes", vrToggle.ControllerClickable);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Toggle Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Toggle to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Toggle", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Toggle", priority = 0)]
        static void InstantiateVRToggle(MenuCommand menuCommand)
        {
            vrTogglePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRToggle.prefab");

            // Create a custom game object
            GameObject newButton = PrefabUtility.InstantiatePrefab(vrTogglePrefab) as GameObject;

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