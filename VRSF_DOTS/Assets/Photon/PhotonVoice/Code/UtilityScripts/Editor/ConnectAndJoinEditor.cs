namespace Photon.Voice.Unity.UtilityScripts.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ConnectAndJoin))]
    public class ConnectAndJoinEditor : Editor
    {
        private ConnectAndJoin connectAndJoin;
        private SerializedProperty randomRoomSp;
        private SerializedProperty roomNameSp;
        private SerializedProperty autoConnectSp;
        private SerializedProperty versionSp;
        private SerializedProperty autoTransmitSp;

        private void OnEnable()
        {
            connectAndJoin = target as ConnectAndJoin;
            randomRoomSp = serializedObject.FindProperty("RandomRoom");
            roomNameSp = serializedObject.FindProperty("RoomName");
            autoConnectSp = serializedObject.FindProperty("autoConnect");
            versionSp = serializedObject.FindProperty("version");
            autoTransmitSp = serializedObject.FindProperty("autoTransmit");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(autoConnectSp);
            EditorGUILayout.PropertyField(versionSp);
            EditorGUILayout.PropertyField(autoTransmitSp);
            EditorGUILayout.PropertyField(randomRoomSp);
            if (!randomRoomSp.boolValue)
            {
                EditorGUILayout.PropertyField(roomNameSp);
            }
            if (Application.isPlaying && !connectAndJoin.IsConnected)
            {
                if (GUILayout.Button("Connect"))
                {
                    connectAndJoin.ConnectNow();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}