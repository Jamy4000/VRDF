using UnityEditor;

namespace VRSF.Core.VRInteractions
{
    [CustomEditor(typeof(PointerClickAuthoring), true)]
    public class PointerClickEditorScript : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            EditorGUILayout.HelpBox("Used to click on VR Objects like the VRSF UI Extension. Our advice is to use the trigger as the interaction button in the VR Interaction Authoring component.", MessageType.Info);
        }
    }
}