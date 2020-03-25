namespace Photon.Voice.Unity.Editor
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Unity;
    using Realtime;

    [CustomEditor(typeof(VoiceConnection))]
    public class VoiceConnectionEditor : Editor
    {
        private VoiceConnection connection;

        private SerializedProperty updateIntervalSp;
        private SerializedProperty enableSupportLoggerSp;
        private SerializedProperty settingsSp;
        #if !UNITY_ANDROID && !UNITY_IOS
        private SerializedProperty runInBackground;
        #endif
        #if !UNITY_IOS
        private SerializedProperty keepAliveInBackgroundSp;
        #endif
        private SerializedProperty applyDontDestroyOnLoadSp;
        private SerializedProperty statsResetInterval;
        private SerializedProperty primaryRecorderSp;
        private SerializedProperty speakerPrefabSp;

        protected virtual void OnEnable()
        {
            connection = target as VoiceConnection;
            updateIntervalSp = serializedObject.FindProperty("updateInterval");
            enableSupportLoggerSp = serializedObject.FindProperty("enableSupportLogger");
            settingsSp = serializedObject.FindProperty("Settings");
            #if !UNITY_ANDROID && !UNITY_IOS
            runInBackground = serializedObject.FindProperty("runInBackground");
            #endif
            #if !UNITY_IOS
            keepAliveInBackgroundSp = serializedObject.FindProperty("KeepAliveInBackground");
            #endif
            applyDontDestroyOnLoadSp = serializedObject.FindProperty("ApplyDontDestroyOnLoad");
            statsResetInterval = serializedObject.FindProperty("statsResetInterval");
            this.primaryRecorderSp = this.serializedObject.FindProperty("primaryRecorder");
            if (this.primaryRecorderSp == null) // [FormerlySerializedAs("PrimaryRecorder")]
            {
                this.primaryRecorderSp = this.serializedObject.FindProperty("PrimaryRecorder");
            }
            this.speakerPrefabSp = this.serializedObject.FindProperty("speakerPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            VoiceLogger.ExposeLogLevel(serializedObject, connection);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(updateIntervalSp, new GUIContent("Update Interval (ms)", "time [ms] between consecutive SendOutgoingCommands calls"));
            EditorGUILayout.PropertyField(enableSupportLoggerSp);
            #if !UNITY_ANDROID && !UNITY_IOS
            EditorGUILayout.PropertyField(runInBackground, new GUIContent("Run In Background", "Sets Unity's Application.runInBackground: Should the application keep running when the application is in the background?"));
            #endif
            #if !UNITY_IOS
            EditorGUILayout.PropertyField(keepAliveInBackgroundSp, new GUIContent("Background Timeout (ms)", "Defines for how long the Fallback Thread should keep the connection, before it may time out as usual."));
            #endif
            EditorGUILayout.PropertyField(applyDontDestroyOnLoadSp, new GUIContent("Don't Destroy On Load", "Persists the GameObject across scenes using Unity's GameObject.DontDestroyOnLoad"));
            if (Application.isPlaying)
            {
                connection.PrimaryRecorder = EditorGUILayout.ObjectField(
                    new GUIContent("Primary Recorder", "Main Recorder to be used for transmission by default"),
                    connection.PrimaryRecorder, typeof(Recorder), true) as Recorder;
                EditorGUILayout.HelpBox("Speaker prefab needs to have a Speaker component in the hierarchy.", MessageType.Info);
                connection.SpeakerPrefab = EditorGUILayout.ObjectField(new GUIContent("Speaker Prefab",
                        "Prefab that contains Speaker component to be instantiated when receiving a new remote audio source info"), connection.SpeakerPrefab, 
                    typeof(GameObject), false) as GameObject;
            }
            else
            {
                EditorGUILayout.PropertyField(this.primaryRecorderSp,
                    new GUIContent("Primary Recorder", "Main Recorder to be used for transmission by default"));
                EditorGUILayout.HelpBox("Speaker prefab needs to have a Speaker component in the hierarchy.", MessageType.Info);
                GameObject prefab = this.speakerPrefabSp.objectReferenceValue as GameObject;
                prefab = EditorGUILayout.ObjectField(new GUIContent("Speaker Prefab",
                        "Prefab that contains Speaker component to be instantiated when receiving a new remote audio source info"), prefab, 
                    typeof(GameObject), false) as GameObject;
                if (prefab == null || prefab.GetComponentInChildren<Speaker>() != null)
                {
                    this.speakerPrefabSp.objectReferenceValue = prefab;
                }
                else
                {
                    Debug.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.", this);
                }
            }
            this.DisplayAppSettings();
            EditorGUILayout.PropertyField(statsResetInterval, new GUIContent("Stats Reset Interval (ms)", "time [ms] between statistics calculations"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(connection.gameObject))
            {
                this.DisplayVoiceStats();
                this.DisplayDebugInfo(this.connection.Client);
                this.DisplayCachedVoiceInfo();
                this.DisplayTrafficStats(this.connection.Client.LoadBalancingPeer);
            }
        }

        private bool showVoiceStats;
        private bool showPlayersList;
        private bool showDebugInfo;
        private bool showCachedVoices;
        private bool showTrafficStats;

        protected virtual void DisplayVoiceStats()
        {
            showVoiceStats =
                EditorGUILayout.Foldout(showVoiceStats, new GUIContent("Voice Frames Stats", "Show frames stats"));
            if (showVoiceStats)
            {
                this.DrawLabel("Frames Received /s", connection.FramesReceivedPerSecond.ToString());
                this.DrawLabel("Frames Lost /s", connection.FramesLostPerSecond.ToString());
                this.DrawLabel("Frames Lost %", connection.FramesLostPercent.ToString());
            }
        }

        protected virtual void DisplayDebugInfo(LoadBalancingClient client)
        {
            showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, new GUIContent("Client Debug Info", "Debug info for Photon client"));
            if (showDebugInfo)
            {
                EditorGUI.indentLevel++;
                this.DrawLabel("Client State", client.State.ToString());
                if (!string.IsNullOrEmpty(client.AppId))
                {
                    this.DrawLabel("AppId", client.AppId);
                }
                if (!string.IsNullOrEmpty(client.AppVersion))
                {
                    this.DrawLabel("AppVersion", client.AppVersion);
                }
                if (!string.IsNullOrEmpty(client.CloudRegion))
                {
                    this.DrawLabel("Current Cloud Region", client.CloudRegion);
                }
                if (client.IsConnected)
                {
                    this.DrawLabel("Current Server Address", client.CurrentServerAddress);
                }
                if (client.InRoom)
                {
                    this.DrawLabel("Room Name", client.CurrentRoom.Name);
                    showPlayersList = EditorGUILayout.Foldout(showPlayersList, new GUIContent("Players List", "List of players joined to the room"));
                    if (showPlayersList)
                    {
                        EditorGUI.indentLevel++;
                        foreach (Player player in client.CurrentRoom.Players.Values)
                        {
                            this.DisplayPlayerDebugInfo(player);
                            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DisplayPlayerDebugInfo(Player player)
        {
            this.DrawLabel("Actor Number", player.ActorNumber.ToString());
            if (!string.IsNullOrEmpty(player.UserId))
            {
                this.DrawLabel("UserId", player.UserId);
            }
            if (!string.IsNullOrEmpty(player.NickName))
            {
                this.DrawLabel("NickName", player.NickName);
            }
            if (player.IsMasterClient)
            {
                EditorGUILayout.LabelField("Master Client");
            }
            if (player.IsLocal)
            {
                EditorGUILayout.LabelField("Local");
            }
            if (player.IsInactive)
            {
                EditorGUILayout.LabelField("Inactive");
            }
        }

        protected virtual void DisplayCachedVoiceInfo()
        {
            showCachedVoices =
                EditorGUILayout.Foldout(showCachedVoices, new GUIContent("Cached Remote Voices' Info", "Show remote voices info cached by local client"));
            if (showCachedVoices)
            {
                List<RemoteVoiceLink> cachedVoices = this.connection.CachedRemoteVoices;
                Speaker[] speakers = FindObjectsOfType<Speaker>();
                for (int i = 0; i < cachedVoices.Count; i++)
                {
                    //VoiceInfo info = cachedVoices[i].Info;
                    EditorGUI.indentLevel++;
                    this.DrawLabel("Voice #", cachedVoices[i].VoiceId.ToString());
                    this.DrawLabel("Player #", cachedVoices[i].PlayerId.ToString());
                    this.DrawLabel("Channel #", cachedVoices[i].ChannelId.ToString());
                    bool linked = false;
                    for (int j = 0; j < speakers.Length; j++)
                    {
                        Speaker speaker = speakers[j];
                        if (speaker.IsLinked && speaker.RemoteVoiceLink.PlayerId == cachedVoices[i].PlayerId &&
                            speaker.RemoteVoiceLink.VoiceId == cachedVoices[i].VoiceId)
                        {
                            linked = true;
                            EditorGUILayout.ObjectField(new GUIContent("Linked Speaker"), speaker, typeof(Speaker), false);
                            break;
                        }
                    }
                    if (!linked)
                    {
                        EditorGUILayout.LabelField("Not Linked");
                    }
                    EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                    EditorGUI.indentLevel--;
                }
            }
        }

        // inspired by PhotonVoiceStatsGui.TrafficStatsWindow
        protected virtual void DisplayTrafficStats(LoadBalancingPeer peer)
        {
            showTrafficStats = EditorGUILayout.Foldout(showTrafficStats, new GUIContent("Traffic Stats", "Traffic Statistics for Photon Client"));
            if (showTrafficStats)
            {
                peer.TrafficStatsEnabled = EditorGUILayout.Toggle(new GUIContent("Enabled", "Enable or disable traffic Statistics for Photon Peer"), peer.TrafficStatsEnabled);
                if (peer.TrafficStatsEnabled)
                {
                    GUILayout.Box("Game Level Stats");
                    var gls = peer.TrafficStatsGameLevel;
                    long elapsedSeconds = peer.TrafficStatsElapsedMs / 1000;
                    if (elapsedSeconds == 0)
                    {
                        elapsedSeconds = 1;
                    }
                    GUILayout.Label(string.Format("Time elapsed: {0} seconds", elapsedSeconds));
                    GUILayout.Label(string.Format("Total: Out {0,4} | In {1,4} | Sum {2,4}", 
                        gls.TotalOutgoingMessageCount, 
                        gls.TotalIncomingMessageCount, 
                        gls.TotalMessageCount));
                    GUILayout.Label(string.Format("Average: Out {0,4} | In {1,4} | Sum {2,4}", 
                        gls.TotalOutgoingMessageCount / elapsedSeconds, 
                        gls.TotalIncomingMessageCount / elapsedSeconds, 
                        gls.TotalMessageCount / elapsedSeconds));
                    GUILayout.Box("Packets Stats");
                    GUILayout.Label(string.Concat("Incoming: \n", peer.TrafficStatsIncoming));
                    GUILayout.Label(string.Concat("Outgoing: \n", peer.TrafficStatsOutgoing));
                    GUILayout.Box("Health Stats");
                    GUILayout.Label(string.Format("ping: {0}[+/-{1}]ms resent:{2}", 
                        peer.RoundTripTime,
                        peer.RoundTripTimeVariance,
                        peer.ResentReliableCommands));
                    GUILayout.Label(string.Format(
                        "max ms between\nsend: {0,4} \ndispatch: {1,4} \nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms",
                        gls.LongestDeltaBetweenSending,
                        gls.LongestDeltaBetweenDispatching,
                        gls.LongestEventCallback,
                        gls.LongestEventCallbackCode,
                        gls.LongestOpResponseCallback,
                        gls.LongestOpResponseCallbackOpCode));
                    if (GUILayout.Button("Reset"))
                    {
                        peer.TrafficStatsReset();
                    }
                }
            }
        }

        private void DrawLabel(string prefix, string text)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(prefix);
            EditorGUILayout.LabelField(text);
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DisplayAppSettings()
        {
            connection.ShowSettings = EditorGUILayout.Foldout(connection.ShowSettings, new GUIContent("Settings", "Settings to be used by this voice connection"));
            if (connection.ShowSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                SerializedProperty sP = settingsSp.FindPropertyRelative("AppIdVoice");
                EditorGUILayout.PropertyField(sP);
                string appId = sP.stringValue;
                string url = "https://dashboard.photonengine.com/en-US/PublicCloud";
                if (!string.IsNullOrEmpty(appId))
                {
                    url = string.Format("https://dashboard.photonengine.com/en-US/App/Manage/{0}", appId);
                }
                if (GUILayout.Button("Dashboard", EditorStyles.miniButton, GUILayout.Width(70)))
                {
                    Application.OpenURL(url);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("AppVersion"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("UseNameServer"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("FixedRegion"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Server"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Port"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("Protocol"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("EnableLobbyStatistics"));
                EditorGUILayout.PropertyField(settingsSp.FindPropertyRelative("NetworkLogging"));
                EditorGUI.indentLevel--;
            }
        }
    }
}