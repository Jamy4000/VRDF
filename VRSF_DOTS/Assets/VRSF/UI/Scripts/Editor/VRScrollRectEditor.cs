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
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        private static GameObject vrScrollRectPrefab;

        private VRScrollRect vrScrollRect;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();
            vrScrollRect = (VRScrollRect)target;
        }
        #endregion


        #region PUBLIC_METHODS
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("VRSF Parameters", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            Undo.RecordObject(vrScrollRect.gameObject, "Add BoxCollider");

            if (vrScrollRect.gameObject.GetComponent<BoxCollider>() != null)
            {
                vrScrollRect.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", vrScrollRect.SetColliderAuto);
            }
            else
            {
                EditorGUILayout.LabelField("This option required a BoxCollider Component.", EditorStyles.miniLabel);
                vrScrollRect.SetColliderAuto = false;
                vrScrollRect.SetColliderAuto = EditorGUILayout.ToggleLeft("Set Box Collider Automatically", false);

                // Add a button to replace the collider by a BoxCollider2D
                if (GUILayout.Button("Add BoxCollider"))
                {
                    vrScrollRect.gameObject.AddComponent<BoxCollider>();
                    DestroyImmediate(vrScrollRect.GetComponent<Collider>());
                    vrScrollRect.SetColliderAuto = true;
                }
            }
            
            vrScrollRect.Direction = (EUIDirection)EditorGUILayout.EnumPopup("Direction", vrScrollRect.Direction);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Basic ScrollRect Parameters", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR ScrollView to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR ScrollView", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR ScrollView", priority = 0)]
        static void InstantiateVRScrollRect(MenuCommand menuCommand)
        {
            vrScrollRectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRScrollView.prefab");

            // Create a custom game object
            GameObject newScrollRect = PrefabUtility.InstantiatePrefab(vrScrollRectPrefab) as GameObject;

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