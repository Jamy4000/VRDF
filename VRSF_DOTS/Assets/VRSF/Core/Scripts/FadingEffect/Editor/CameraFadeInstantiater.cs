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
        [MenuItem("GameObject/VRSF/Utils/Add Camera Fader", priority = 2)]
        [MenuItem("VRSF/Utils/Add Camera Fader", priority = 2)]
        private static void AddCameraFader(MenuCommand menuCommand)
        {
            GameObject cameraFader = GameObject.FindObjectOfType<CameraFadeAuthoring>().gameObject;

            if (cameraFader != null)
            {
                Debug.LogError("<b>[VRSF] :</b> a Camera Fader is already present in the scene.\n No need to add multiple instance of it.");
                Selection.activeObject = cameraFader;
                return;
            }

            var faderPrefab = Utils.VRSFPrefabReferencer.GetPrefab("CameraFade");

            // Instantiate a custom game object
            cameraFader = PrefabUtility.InstantiatePrefab(faderPrefab) as GameObject;

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(cameraFader, "Create " + cameraFader.name);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(cameraFader, FindObjectOfType<Camera>().gameObject);

            Selection.activeObject = cameraFader;
        }
    }
}