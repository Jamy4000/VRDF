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
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private VRHandleSlider _handleSlider;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _handleSlider = (VRHandleSlider)target;
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
            Undo.RecordObject(_handleSlider.gameObject, "Add BoxCollider");

            if (_handleSlider.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _handleSlider.SetColliderAuto = false;
                _handleSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_handleSlider.GetComponent<Collider>());
                    _handleSlider.gameObject.AddComponent<BoxCollider>();
                    _handleSlider.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Slider Parameters", EditorStyles.boldLabel);

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
                PrefabUtility.RecordPrefabInstancePropertyModifications(_handleSlider);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Slider with Handle to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/Sliders/VR Handle Slider", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/Sliders/VR Handle Slider", priority = 0)]
        static void InstantiateVRHandleSlider(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newSlider = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRHandleSlider"));

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
    }
}
#endif