using UnityEditor;

namespace VRDF.Core.Raycast
{
    [CustomEditor(typeof(VRRaycastAuthoring))]
    public class VRRaycastAuthoringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (((VRRaycastAuthoring)target).RayOrigin == ERayOrigin.NONE)
                EditorGUILayout.HelpBox("The RayOrigin can't be NONE !", MessageType.Error);
        }
    }
}