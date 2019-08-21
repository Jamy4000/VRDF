using UnityEditor;
using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;

namespace VRSF.Multiplayer
{
    public class MultiplayerPrefabsInstancier : Editor
    {
        /// <summary>
        /// Add a new Lobby Connection Manager to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/Lobby/Connection Manager", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/Lobby/Connection Manager", priority = 0)]
        static void InstantiateConnectionManager(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("ConnectionManager"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        /// <summary>
        /// Add a new Game Manager for the rooms to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/In Room/Game Manager", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/In Room/Game Manager", priority = 0)]
        static void InstantiateGameManager(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("GameManager"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        /// <summary>
        /// Add a new Sahred laser pointer for the rooms to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/In Room/Shared Laser Pointer", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/In Room/Shared Laser Pointer", priority = 0)]
        static void InstantiateSharedLaserPointer(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("LaserPointerMultiplayer"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        /// <summary>
        /// Add a new SetupMultiVR for the rooms to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/In Room/SetupVR Multiplayer", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/In Room/SetupVR Multiplayer", priority = 0)]
        static void InstantiateSetupMultiVR(MenuCommand menuCommand)
        {
            var deviceAuthoring = GameObject.FindObjectOfType<DeviceToLoadAuthoring>();

            if (deviceAuthoring != null)
            {
                Debug.LogError("<b>[VRSF] :</b> SetupVR is already present in the scene.\n" +
                    "If multiple instance of this object are placed in the same scene, you will encounter conflict problems.");
                Selection.activeObject = deviceAuthoring.gameObject;
                return;
            }

            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("SetupMultiVR"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        /// <summary>
        /// Add a Shared Right Controller Model to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/In Room/Shared Right Controller Mesh", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/In Room/Shared Right Controller Mesh", priority = 0)]
        static void InstantiateRightController(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("RightControllerModelMultiplayer"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        /// <summary>
        /// Add a new Right Controller Model for the rooms to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRSF/Multiplayer/In Room/Shared Left Controller Mesh", priority = 0)]
        [MenuItem("GameObject/VRSF/Multiplayer/In Room/Shared Left Controller Mesh", priority = 0)]
        static void InstantiateLeftController(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRSFPrefabReferencer.GetPrefab("LeftControllerModelMultiplayer"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }
    }
}