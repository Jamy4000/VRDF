#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRDF.Core.Utils;

namespace VRDF.UI.Editor
{
    /// <summary>
    /// Create Context menu in hierarchy and in the top tabs to instantiate a new VR Canvas
    /// A VR Canvas is just a Canvas already set for VR
    /// </summary>
	public static class VRCanvasEditor
    {
        /// <summary>
        /// Add a new VR Canvas to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRDF/UI/VR Canvas", priority = 0)]
        [MenuItem("GameObject/VRDF/UI/VR Canvas", priority = 0)]
        static void InstantiateVRCanvas(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newCanvas = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("VRCanvas"));

            RectTransform rt = newCanvas.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = new Vector3(0.012f, 0.012f, 0.012f);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newCanvas, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newCanvas, "Create " + newCanvas.name);
            Selection.activeObject = newCanvas;
        }
    }
}
#endif