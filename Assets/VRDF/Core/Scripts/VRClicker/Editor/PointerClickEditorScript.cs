using UnityEditor;

namespace VRDF.Core.VRClicker
{
    [CustomEditor(typeof(VRClickerAuthoring), true)]
    public class PointerClickEditorScript : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Used to click on VR Objects like the VRDF UI Extension. We recommend to use the trigger as the interaction button in the VR Interaction Authoring component.", MessageType.Info);
        }
    }
}