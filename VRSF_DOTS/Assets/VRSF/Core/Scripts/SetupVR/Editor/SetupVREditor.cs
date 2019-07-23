#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Script to add some Editor feature for the SetupVR GameObject.
    /// </summary>
    [CustomEditor(typeof(DeviceToLoadAuthoring), true)]
    public class SetupVREditor : Editor
    {
        #region PRIVATE_VARIABLES
        private static GameObject _setupVRPrefab;
        #endregion

        #region PRIVATE_METHODS
        /// <summary>
        /// Add the SetupVR Prefab to the scene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Add SetupVR to Scene", priority = 0)]
        [MenuItem("VRSF/Add SetupVR to Scene", priority = 0)]
        private static void InstantiateSetupVR(MenuCommand menuCommand)
        {
            var deviceAuthoring = GameObject.FindObjectOfType<DeviceToLoadAuthoring>();

            if (deviceAuthoring != null)
            {
                Debug.LogError("<b>[VRSF] :</b> SetupVR is already present in the scene.\n" +
                    "If multiple instance of this object are placed in the same scene, you will encounter conflict problems.");
                Selection.activeObject = deviceAuthoring.gameObject;
                return;
            }

            _setupVRPrefab = Utils.VRSFPrefabReferencer.GetPrefab("SetupVR");

            // Create a custom game object
            var setupVR = PrefabUtility.InstantiatePrefab(_setupVRPrefab) as GameObject;

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(setupVR, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(setupVR, "Create " + setupVR.name);
            Selection.activeObject = setupVR;
        }
        #endregion
    }
}
#endif