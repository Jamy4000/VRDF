using UnityEngine;
using UnityEditor;

namespace VRDF.Core.FadingEffect
{
    public class CameraFadeInstantiater : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRDF/Utils/Add Camera Fader", priority = 2)]
        [MenuItem("VRDF/Utils/Add Camera Fader", priority = 2)]
        private static void AddCameraFader(MenuCommand _)
        {
            CameraFadeAuthoring cameraFader = GameObject.FindObjectOfType<CameraFadeAuthoring>();

            if (cameraFader != null)
            {
                Debug.LogError("<b>[VRDF] :</b> a Camera Fader is already present in the scene.\n No need to add multiple instance of it.");
                Selection.activeObject = cameraFader.gameObject;
            }
            else
            {
                var faderPrefab = Utils.VRDFPrefabReferencer.GetPrefab("CameraFade");

                // Instantiate a custom game object
                var newCameraFader = PrefabUtility.InstantiatePrefab(faderPrefab) as GameObject;
                
                // Register the creation in the undo system
                Undo.RegisterCreatedObjectUndo(newCameraFader, "Create " + newCameraFader.name);

                // Ensure it gets reparented if this was a context click (otherwise does nothing)
                GameObjectUtility.SetParentAndAlign(newCameraFader, FindObjectOfType<Camera>().gameObject);

                Selection.activeObject = newCameraFader;
            }
        }
    }
}