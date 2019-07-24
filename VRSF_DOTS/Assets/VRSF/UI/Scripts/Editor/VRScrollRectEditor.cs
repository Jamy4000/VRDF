using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary> 
    /// Add the public variable of VRScrollRect to the Editor and create a MenuItem to add the prefab to the scene
    /// </summary>
    [CustomEditor(typeof(VRScrollRect), true)]
    [CanEditMultipleObjects]
    public class VRScrollRectEditor : UnityEditor.UI.ScrollRectEditor
    {
        #region PRIVATE_VARIABLES
        private VRScrollRect _scrollview;
        private SerializedProperty _setColliderAuto;
        private SerializedProperty _direction;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            _scrollview = (VRScrollRect)target;
            _setColliderAuto = serializedObject.FindProperty("SetColliderAuto");
            _direction = serializedObject.FindProperty("Direction");
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
            Undo.RecordObject(_scrollview.gameObject, "Add BoxCollider");

            if (_scrollview.gameObject.GetComponent<BoxCollider>() != null)
            {
                EditorGUILayout.LabelField("Set Box Collider Automatically", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(_setColliderAuto);
                CheckEndChanges();
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                _scrollview.SetColliderAuto = false;
                _scrollview.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    DestroyImmediate(_scrollview.GetComponent<Collider>());
                    _scrollview.gameObject.AddComponent<BoxCollider>();
                    _scrollview.SetColliderAuto = true;
                }
            }

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(_scrollview, "Direction");
            EditorGUILayout.LabelField("The direction of this ScrollView", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_direction);
            CheckEndChanges();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic ScrollRect Parameters", EditorStyles.boldLabel);

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
                PrefabUtility.RecordPrefabInstancePropertyModifications(_scrollview);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a new VR ScrollView to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR ScrollView", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR ScrollView", priority = 0)]
        static void InstantiateVRScrollRect(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newScrollRect = (GameObject)PrefabUtility.InstantiatePrefab(Core.Utils.VRSFPrefabReferencer.GetPrefab("VRScrollview"));

            RectTransform rt = newScrollRect.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.x, rt.rect.y, 0);
            rt.localScale = Vector3.one;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newScrollRect, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newScrollRect, "Create " + newScrollRect.name);
            Selection.activeObject = newScrollRect;
        }
        #endregion PRIVATE_METHODS
    }
}