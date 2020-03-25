using UnityEditor;
using UnityEngine;
using VRDF.Core.Utils;

namespace VRDF.Core.Controllers
{
    public class ControllersPrefabs : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRDF/Controllers/Left Controllers", priority = 0)]
        [MenuItem("GameObject/VRDF/Controllers/Left Controllers", priority = 0)]
        static void InstantiateLeftControllers(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newController = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("LeftControllerModel [DestroyedOnStart]"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newController, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newController, "Create " + newController.name);
            Selection.activeObject = newController;
        }

        /// <summary>
        /// Add a new Game Manager for the rooms to the Scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("VRDF/Controllers/Right Controllers", priority = 0)]
        [MenuItem("GameObject/VRDF/Controllers/Right Controllers", priority = 0)]
        static void InstantiateRightControllers(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject newController = (GameObject)PrefabUtility.InstantiatePrefab(VRDFPrefabReferencer.GetPrefab("RightControllerModel [DestroyedOnStart]"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newController, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newController, "Create " + newController.name);
            Selection.activeObject = newController;
        }
    }
} 
