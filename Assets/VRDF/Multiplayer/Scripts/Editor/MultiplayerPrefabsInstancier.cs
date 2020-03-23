using UnityEditor;
using UnityEngine;
using VRDF.Core.Utils;

namespace VRDF.Multiplayer
{
    public class MultiplayerPrefabsInstancier : EditorWindow
    {
        private static string _infoToAddToWindow;
        private static MultiplayerPrefabsInstancier _window;

        private void OnGUI()
        {
            // Add a Title
            GUILayout.Label("This prefab is only to be placed under the Photon Player prefab.\n" +
                "In VRDF, it's situated under Assets/VRDF/Multiplayer/Resources/PhotonPrefabs.\n" + _infoToAddToWindow, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            if (GUILayout.Button("Understood !"))
            {
                this.Close();
            }
        }

        /// <summary>
        /// Add a new Connection Manager to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRDF/Multiplayer/Connection Scene/Connection Manager", priority = 0)]
        [MenuItem("GameObject/VRDF/Multiplayer/Connection Scene/Connection Manager", priority = 0)]
        static void InstantiateConnectionManager(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("ConnectionManager"));

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
        [MenuItem("VRDF/Multiplayer/In Room/Game Manager", priority = 0)]
        [MenuItem("GameObject/VRDF/Multiplayer/In Room/Game Manager", priority = 0)]
        static void InstantiateGameManager(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("GameManager"));

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
        [MenuItem("VRDF/Multiplayer/In Room/Players Utility/Shared Remote Laser Pointer", priority = 0)]
        [MenuItem("GameObject/VRDF/Multiplayer/In Room/Players Utility/Shared Remote Laser Pointer", priority = 0)]
        static void InstantiateSharedLaserPointer(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("LaserPointerMultiplayer"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;

            _infoToAddToWindow = "This prefab let you see the laser pointer of the remote users. TO use this feature,\n" +
                "you need to add a Laser Pointer (Hierarchy>VRDF>Raycast>Add Raycaster with Laser Pointer) for the local user in your scene or SetupVR prefab.";
            _window = CreateWindow<MultiplayerPrefabsInstancier>("Laser for Remote Players");
            SetWindowSize();
        }

        /// <summary>
        /// Add a Shared Right Controller Model to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRDF/Multiplayer/In Room/Players Utility/Shared Right Controller Mesh", priority = 0)]
        [MenuItem("GameObject/VRDF/Multiplayer/In Room/Players Utility/Shared Right Controller Mesh", priority = 0)]
        static void InstantiateRightController(MenuCommand menuCommand)
        {
            _infoToAddToWindow = "This prefab let you see the Right controller of the other users.";
            _window = CreateWindow<MultiplayerPrefabsInstancier>("Right Controller for Remote Players");
            SetWindowSize();

            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("RightControllerModelMultiplayer"));

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
        [MenuItem("VRDF/Multiplayer/In Room/Players Utility/Shared Left Controller Mesh", priority = 0)]
        [MenuItem("GameObject/VRDF/Multiplayer/In Room/Players Utility/Shared Left Controller Mesh", priority = 0)]
        static void InstantiateLeftController(MenuCommand menuCommand)
        {
            _infoToAddToWindow = "This prefab let you see the Left controller of the other users.";
            _window = CreateWindow<MultiplayerPrefabsInstancier>("Left Controller for Remote Players");
            SetWindowSize();

            // Create a custom game object
            GameObject newMultiObject = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("LeftControllerModelMultiplayer"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newMultiObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newMultiObject, "Create " + newMultiObject.name);
            Selection.activeObject = newMultiObject;
        }

        private static void SetWindowSize()
        {
            _window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            _window.minSize = new Vector2(800.0f, 120.0f);
        }
    }
}