using UnityEditor;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Simply to grey out the player field, as it's set in scripts
    /// </summary>
    [CustomEditor(typeof(VRDFPlayerManager))]
    public class VDRFPlayerManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
        }
    }
}