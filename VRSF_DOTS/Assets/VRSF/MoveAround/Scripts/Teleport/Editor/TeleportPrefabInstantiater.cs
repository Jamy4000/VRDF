using UnityEngine;
using UnityEditor;
using VRSF.Core.Utils;

namespace VRSF.MoveAround.Teleport
{
    public class TeleportPrefabInstantiater : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Teleport/Step by Step", priority = 1)]
        [MenuItem("VRSF/Move Around/Teleport/Step by Step", priority = 1)]
        private static void AddBasicPointer(MenuCommand menuCommand)
        {
            var pointerPrefab = VRSFPrefabReferencer.GetPrefab("StepByStepTeleporter");
            CreateGameObject(pointerPrefab, menuCommand);
        }

        private static void CreateGameObject(GameObject pointerPrefab, MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject pointer = PrefabUtility.InstantiatePrefab(pointerPrefab) as GameObject;

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(pointer, "Create " + pointer.name);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(pointer, menuCommand.context as GameObject);

            Selection.activeObject = pointer;
        }
    }
}