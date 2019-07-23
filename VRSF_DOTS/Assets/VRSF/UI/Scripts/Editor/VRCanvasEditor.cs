#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Canvas
    /// A VR Canvas is just a Canvas already set for VR
    /// </summary>
	public static class VRCanvasEditor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private static GameObject vrCanvas;
        #endregion

        // EMPTY
        #region MONOBEHAVIOUR_METHODS
        #endregion

        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add a new VR Canvas to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/UI/VR Canvas", priority = 0)]
        [MenuItem("GameObject/VRSF/UI/VR Canvas", priority = 0)]
        static void InstantiateVRCanvas(MenuCommand menuCommand)
        {
            vrCanvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/UI/UIElements/VRCanvas.prefab");

            // Create a custom game object
            GameObject newCanvas = PrefabUtility.InstantiatePrefab(vrCanvas) as GameObject;

            RectTransform rt = newCanvas.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = new Vector3(0.012f, 0.012f, 0.012f);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newCanvas, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newCanvas, "Create " + newCanvas.name);
            Selection.activeObject = newCanvas;
        }
        #endregion

        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}
#endif