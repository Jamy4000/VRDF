using UnityEngine;
using UnityEditor;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeInstantiater : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Utils/Add Camera Fader", priority = 1)]
        [MenuItem("VRSF/Utils/Add Camera Fader", priority = 1)]
        private static void AddCameraFader(MenuCommand menuCommand)
        {
            var faderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRSF/Prefabs/Core/CameraFade/CameraFade.prefab");

            // Create a custom game object
            GameObject pointer = PrefabUtility.InstantiatePrefab(faderPrefab) as GameObject;

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(pointer, "Create " + pointer.name);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(pointer, FindObjectOfType<Camera>().gameObject);

            Selection.activeObject = pointer;
        }
    }
}