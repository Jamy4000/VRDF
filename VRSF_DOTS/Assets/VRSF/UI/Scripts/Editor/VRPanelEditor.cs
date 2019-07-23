#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Panel
    /// A VR Panel is just a Canvas already set for VR with a Panel as Child
    /// </summary>
	public static class VRPanelEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrPanel;
        #endregion

        // EMPTY
        #region MONOBEHAVIOUR_METHODS
        #endregion

        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Panel to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Panel", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Panel", priority = 0)]
        static void InstantiateVRPanel(MenuCommand menuCommand)
        {
            vrPanel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRPanel.prefab");

            // Create a custom game object
            GameObject newPanel = PrefabUtility.InstantiatePrefab(vrPanel) as GameObject;

            RectTransform rt = newPanel.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = new Vector3(0.012f, 0.012f, 0.012f);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newPanel, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newPanel, "Create " + newPanel.name);
            Selection.activeObject = newPanel;
        }
        #endregion

        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif