#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Handle Slider
    /// </summary>
    [CustomEditor(typeof(VRHandleSlider), true)]
    [CanEditMultipleObjects]
    public class VRHandleSliderEditor : UnityEditor.UI.SliderEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrHandleSliderPrefab;

        private VRHandleSlider vrHandleSlider;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            vrHandleSlider = (VRHandleSlider)target;
        }
        #endregion

        
        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrHandleSlider.gameObject, "Add BoxCollider");

            if (vrHandleSlider.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrHandleSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrHandleSlider.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrHandleSlider.SetColliderAuto = false;
                vrHandleSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrHandleSlider.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrHandleSlider.GetComponent<Collider>());
                    vrHandleSlider.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Slider Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Slider with Handle to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/Sliders/VR Handle Slider", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/Sliders/VR Handle Slider", priority = 0)]
        static void InstantiateVRHandleSlider(MenuCommand menuCommand)
        {
            vrHandleSliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRHandleSlider.prefab");

            // Create a custom game object
            GameObject newSlider = PrefabUtility.InstantiatePrefab(vrHandleSliderPrefab) as GameObject;

            RectTransform rt = newSlider.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newSlider, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newSlider, "Create " + newSlider.name);
            Selection.activeObject = newSlider;
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif