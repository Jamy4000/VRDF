#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRSF.Core.Utils;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Add the public variable of VRAutoSlider to the Editor and add a MenuItem to add an autoFillSlider to the scene
    /// </summary>
    [CustomEditor(typeof(VRAutoFillSlider), true)]
    [CanEditMultipleObjects]
    public class VRAutoFillSliderEditor : UnityEditor.UI.SliderEditor
    {
        #region PRIVATE_VARIABLES
        private SerializedProperty _setColliderAuto;
        private SerializedProperty _clickableWithRaycast;
        private SerializedProperty _clickableUsingControllers;
        private SerializedProperty _valueGoingDown;
        private SerializedProperty _resetFillOnRelease;
        private SerializedProperty _fillWithClick;
        private SerializedProperty _fillTime;

        private SerializedProperty m_OnBarFilled;
        private SerializedProperty m_OnBarReleased;

        private VRAutoFillSlider _autoSlider;
        #endregion PRIVATE_VARIABLES


        protected override void OnEnable()
        {
            base.OnEnable();

            _autoSlider = (VRAutoFillSlider)target;

            _setColliderAuto = serializedObject.FindProperty("SetColliderAuto");
            _clickableWithRaycast = serializedObject.FindProperty("LaserClickable");
            _clickableUsingControllers = serializedObject.FindProperty("ControllerClickable");
            _valueGoingDown = serializedObject.FindProperty("ValueIsGoingDown");
            _resetFillOnRelease = serializedObject.FindProperty("ResetFillOnRelease");
            _fillWithClick = serializedObject.FindProperty("FillWithClick");
            _fillTime = serializedObject.FindProperty("FillTime");

            m_OnBarFilled = serializedObject.FindProperty("OnBarFilled");
            m_OnBarReleased = serializedObject.FindProperty("OnBarReleased");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "Add BoxCollider");

            if (_autoSlider.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _autoSlider.SetColliderAuto = false;
                _autoSlider.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);
                
                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_autoSlider.GetComponent<Collider>());
                    _autoSlider.gameObject.AddComponent<BoxCollider>();
                    _autoSlider.SetColliderAuto = true;
                }
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "LaserClickable");
            EditorGUILayout.LabelField("Clickable using Laser Pointer with Click", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableWithRaycast);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "MeshClickable");
            EditorGUILayout.LabelField("Clickable using Controllers' meshes", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_clickableUsingControllers);
            CheckEndChanges();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "ValueGoingDown");
            EditorGUILayout.LabelField("Should the value decrease when not interacting", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_valueGoingDown);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "ResetFill");
            EditorGUILayout.LabelField("Should the value be reset On Release", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_resetFillOnRelease);
            CheckEndChanges();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "FillWithClick");
            EditorGUILayout.LabelField("Should the user click to Fill the Slider", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_fillWithClick);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "FillTime");
            EditorGUILayout.LabelField("Time to Fill the slider", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_fillTime);
            CheckEndChanges();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic Slider Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "m_OnBarFilled");
            EditorGUILayout.PropertyField(m_OnBarFilled);
            CheckEndChanges();

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_autoSlider, "m_OnBarReleased");
            EditorGUILayout.PropertyField(m_OnBarReleased);
            CheckEndChanges();
        }

        private bool CheckEndChanges()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                PrefabUtility.RecordPrefabInstancePropertyModifications(_autoSlider);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR Auto Filling Slider to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/Sliders/VR AutoFill Slider", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/Sliders/VR AutoFill Slider", priority = 0)]
        static void InstantiateVRAutoFillSlider(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newSlider = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("VRAutoFillSlider"));

            RectTransform rt = newSlider.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newSlider, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newSlider, "Create " + newSlider.name);
            Selection.activeObject = newSlider;
        }
    }
}
#endif