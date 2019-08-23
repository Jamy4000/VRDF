using UnityEditor;
using UnityEngine;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyModeAuthoring))]
    [CanEditMultipleObjects]
    public class FlyComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("To use the Fly System, you only need to set the parameters of the Fly Mode Authoring and the VRInteractions Authoring.\n" +
                "You have to use the Touchpad as button and UP/DOWN as Touch position. All you need to specify in the VRInteractionAuthoring are the Interaction Type and the Button Hand.", MessageType.Info);
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Fly Mode/Without Boundaries", priority = 3)]
        [MenuItem("VRSF/Move Around/Fly Mode/Without Boundaries", priority = 3)]
        public static void AddFlyModeWithoutBoundaries(MenuCommand menuCommand)
        {
            var flyModeObject = BasicFlyGoSetup("Fly Mode");
            Selection.SetActiveObjectWithContext(flyModeObject, menuCommand.context);
        }

        /// <summary>
        /// Add a linear rotation gameObject in the scene
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/VRSF/Move Around/Fly Mode/With Boundaries", priority = 3)]
        [MenuItem("VRSF/Move Around/Fly Mode/With Boundaries", priority = 3)]
        public static void AddFlyModeWithBoundaries(MenuCommand menuCommand)
        {
            var flyModeObject = BasicFlyGoSetup("Restricted Fly Mode");
            flyModeObject.AddComponent<FlyBoundariesAuthoring>();
            Selection.SetActiveObjectWithContext(flyModeObject, menuCommand.context);
        }

        private static GameObject BasicFlyGoSetup(string name)
        {
            // Create GO
            var flyModeObject = new GameObject(name);

            // Add Undo Response
            Undo.RegisterCreatedObjectUndo(flyModeObject, "Adding fly mode");

            // Reset Transform and set parent to the currently selected transform
            flyModeObject.transform.SetParent(Selection.activeTransform);
            flyModeObject.transform.position = Vector3.zero;
            flyModeObject.transform.rotation = Quaternion.identity;

            // Add FlyModeAuthoring script
            flyModeObject.AddComponent<FlyModeAuthoring>();

            // Setup the interaction script
            SetupInteractions(flyModeObject);
            return flyModeObject;
        }

        /// <summary>
        /// Setup the interaction script according to the flying mode requirements
        /// </summary>
        /// <param name="flyModeObject">the objects we need to set</param>
        private static void SetupInteractions(GameObject flyModeObject)
        {
            var interactions = flyModeObject.GetComponent<VRInteractionAuthoring>();
            interactions.ButtonToUse = Core.Inputs.EControllersButton.TOUCHPAD;
            interactions.ClickThumbPosition = Core.Inputs.EThumbPosition.DOWN | Core.Inputs.EThumbPosition.UP;
            interactions.TouchThumbPosition = Core.Inputs.EThumbPosition.DOWN | Core.Inputs.EThumbPosition.UP;
            interactions.IsClickingThreshold = 0.0f;
            interactions.IsTouchingThreshold = 0.0f;
        }
    }
}