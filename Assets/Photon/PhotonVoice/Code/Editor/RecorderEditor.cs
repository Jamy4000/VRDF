#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
#define PHOTON_MICROPHONE_ENUMERATOR
#endif

namespace Photon.Voice.Unity.Editor
{
    using System;
    #if PHOTON_MICROPHONE_ENUMERATOR
    using System.Collections.Generic;
    #endif
    using Unity;
    using UnityEditor;
    using UnityEngine;
    #if UNITY_IOS
    using IOS;
    #endif

    [CustomEditor(typeof(Recorder))]
    public class RecorderEditor : Editor
    {
        private Recorder recorder;

        private int unityMicrophoneDeviceIndex;

        #if PHOTON_MICROPHONE_ENUMERATOR
        private string[] photonDeviceNames;
        private int[] photonDeviceIDs;
        private int photonDeviceIndex;
        #endif

        private int calibrationTime = 200;

        private SerializedProperty voiceDetectionSp;
        private SerializedProperty voiceDetectionThresholdSp;
        private SerializedProperty voiceDetectionDelayMsSp;
        private SerializedProperty unityMicrophoneDeviceSp;
        private SerializedProperty photonMicrophoneDeviceIdSp;
        private SerializedProperty interestGroupSp;
        private SerializedProperty debugEchoModeSp;
        private SerializedProperty reliableModeSp;
        private SerializedProperty encryptSp;
        private SerializedProperty transmitEnabledSp;
        private SerializedProperty samplingRateSp;
        private SerializedProperty frameDurationSp;
        private SerializedProperty bitrateSp;
        private SerializedProperty sourceTypeSp;
        private SerializedProperty microphoneTypeSp;
        private SerializedProperty audioClipSp;
        private SerializedProperty loopAudioClipSp;
        private SerializedProperty reactOnSystemChangesSp;
        private SerializedProperty autoStartSp;

        #if UNITY_IOS
        private SerializedProperty useCustomAudioSessionParametersSp;
        private SerializedProperty audioSessionParametersSp;
        private SerializedProperty audioSessionPresetIndexSp;
        private SerializedProperty audioSessionParametersCategorySp;
        private SerializedProperty audioSessionParametersModeSp;
        private SerializedProperty audioSessionParametersCategoryOptionsSp;

        private string[] iOSAudioSessionPresetsNames = {"Game", "VoIP"};
        private AudioSessionParameters[] iOSAudioSessionPresetsValues =
            {AudioSessionParametersPresets.Game, AudioSessionParametersPresets.VoIP};
        #endif

        private void OnEnable()
        {
            recorder = target as Recorder;
            AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
            this.RefreshMicrophones();
            this.voiceDetectionSp = this.serializedObject.FindProperty("voiceDetection");
            this.voiceDetectionThresholdSp = this.serializedObject.FindProperty("voiceDetectionThreshold");
            this.voiceDetectionDelayMsSp = this.serializedObject.FindProperty("voiceDetectionDelayMs");
            this.unityMicrophoneDeviceSp = this.serializedObject.FindProperty("unityMicrophoneDevice");
            this.photonMicrophoneDeviceIdSp = this.serializedObject.FindProperty("photonMicrophoneDeviceId");
            this.interestGroupSp = this.serializedObject.FindProperty("interestGroup");
            this.debugEchoModeSp = this.serializedObject.FindProperty("debugEchoMode");
            this.reliableModeSp = this.serializedObject.FindProperty("reliableMode");
            this.encryptSp = this.serializedObject.FindProperty("encrypt");
            this.transmitEnabledSp = this.serializedObject.FindProperty("transmitEnabled");
            this.samplingRateSp = this.serializedObject.FindProperty("samplingRate");
            this.frameDurationSp = this.serializedObject.FindProperty("frameDuration");
            this.bitrateSp = this.serializedObject.FindProperty("bitrate");
            this.sourceTypeSp = this.serializedObject.FindProperty("sourceType");
            this.microphoneTypeSp = this.serializedObject.FindProperty("microphoneType");
            this.audioClipSp = this.serializedObject.FindProperty("audioClip");
            this.loopAudioClipSp = this.serializedObject.FindProperty("loopAudioClip");
            this.reactOnSystemChangesSp = this.serializedObject.FindProperty("reactOnSystemChanges");
            this.autoStartSp = this.serializedObject.FindProperty("autoStart");
            #if UNITY_IOS
            useCustomAudioSessionParametersSp = serializedObject.FindProperty("useCustomAudioSessionParameters");
            audioSessionPresetIndexSp = serializedObject.FindProperty("audioSessionPresetIndex");
            audioSessionParametersSp = serializedObject.FindProperty("audioSessionParameters");
            audioSessionParametersCategorySp = audioSessionParametersSp.FindPropertyRelative("Category");
            audioSessionParametersModeSp = audioSessionParametersSp.FindPropertyRelative("Mode");
            audioSessionParametersCategoryOptionsSp = audioSessionParametersSp.FindPropertyRelative("CategoryOptions");
            #endif
        }

        private void OnDisable()
        {
            AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            //serializedObject.Update();

            if (PhotonVoiceEditorUtils.IsInTheSceneInPlayMode(recorder.gameObject))
            {
                if (recorder.RequiresRestart)
                {
                    EditorGUILayout.HelpBox("Recorder requires restart. Call Recorder.RestartRecording().", MessageType.Warning);
                    if (GUILayout.Button("RestartRecording"))
                    {
                        recorder.RestartRecording();
                    }
                }
                else if (!recorder.IsInitialized)
                {
                    EditorGUILayout.HelpBox("Recorder requires initialization. Call Recorder.Init or VoiceConnection.InitRecorder.", MessageType.Warning);
                }
            }
            VoiceLogger.ExposeLogLevel(serializedObject, recorder);

            EditorGUI.BeginChangeCheck();
            if (Application.isPlaying)
            {
                recorder.ReactOnSystemChanges = EditorGUILayout.Toggle(new GUIContent("React On System Changes", "If true, recording is restarted when Unity detects Audio Config. changes."), recorder.ReactOnSystemChanges);
                recorder.TransmitEnabled = EditorGUILayout.Toggle(new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."), recorder.TransmitEnabled);
                if (recorder.IsInitialized)
                {
                    recorder.IsRecording = EditorGUILayout.Toggle(new GUIContent("IsRecording", "If true, audio recording is on."), recorder.IsRecording);
                }
                else
                {
                    EditorGUILayout.PropertyField(this.autoStartSp,
                        new GUIContent("Auto Start", "If true, recording is started when Recorder is initialized."));
                }
                if (recorder.IsRecording && recorder.TransmitEnabled)
                {
                    float amplitude = 0f;
                    if (recorder.IsCurrentlyTransmitting)
                    {
                        amplitude = recorder.LevelMeter.CurrentPeakAmp;
                    }
                    EditorGUILayout.Slider("Level", amplitude, 0, 1);
                }
                recorder.Encrypt = EditorGUILayout.Toggle(new GUIContent("Encrypt", "If true, voice stream is sent encrypted."), recorder.Encrypt);
                recorder.InterestGroup = (byte)EditorGUILayout.IntField(new GUIContent("Interest Group", "Target interest group that will receive transmitted audio."), recorder.InterestGroup);
                if (recorder.InterestGroup == 0)
                {
                    recorder.DebugEchoMode = EditorGUILayout.Toggle(new GUIContent("Debug Echo", "If true, outgoing stream routed back to client via server same way as for remote client's streams."), recorder.DebugEchoMode);
                }
                recorder.ReliableMode = EditorGUILayout.Toggle(new GUIContent("Reliable Mode", "If true, stream data sent in reliable mode."), recorder.ReliableMode);

                EditorGUILayout.LabelField("Codec Parameters", EditorStyles.boldLabel);
                recorder.FrameDuration = (OpusCodec.FrameDuration)EditorGUILayout.EnumPopup(new GUIContent("Frame Duration", "Outgoing audio stream encoder delay."), recorder.FrameDuration);
                recorder.SamplingRate = (POpusCodec.Enums.SamplingRate)EditorGUILayout.EnumPopup(
                    new GUIContent("Sampling Rate", "Outgoing audio stream sampling rate."), recorder.SamplingRate);
                recorder.Bitrate = EditorGUILayout.IntField(new GUIContent("Bitrate", "Outgoing audio stream bitrate."),
                    recorder.Bitrate);

                EditorGUILayout.LabelField("Audio Source Settings", EditorStyles.boldLabel);
                recorder.SourceType = (Recorder.InputSourceType) EditorGUILayout.EnumPopup(new GUIContent("Input Source Type", "Input audio data source type"), recorder.SourceType);
                switch (recorder.SourceType)
                {
                    case Recorder.InputSourceType.Microphone:
                        recorder.MicrophoneType = (Recorder.MicType) EditorGUILayout.EnumPopup(
                            new GUIContent("Microphone Type",
                                "Which microphone API to use when the Source is set to Microphone."),
                            recorder.MicrophoneType);
                        EditorGUILayout.HelpBox("Devices list and current selection is valid in Unity Editor only. In build, you need to set it via code preferably at runtime.", MessageType.Info);
                        switch (recorder.MicrophoneType)
                        {
                            case Recorder.MicType.Unity:
                                if (Microphone.devices.Length == 0)
                                {
                                    EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                }
                                else
                                {
                                    unityMicrophoneDeviceIndex = EditorGUILayout.Popup("Microphone Device", unityMicrophoneDeviceIndex, Microphone.devices);
                                    recorder.UnityMicrophoneDevice = Microphone.devices[unityMicrophoneDeviceIndex];
                                    int minFreq, maxFreq;
                                    Microphone.GetDeviceCaps(Microphone.devices[unityMicrophoneDeviceIndex], out minFreq, out maxFreq);
                                    EditorGUILayout.LabelField("Microphone Device Caps", string.Format("{0}..{1} Hz", minFreq, maxFreq));
                                }
                                break;
                            case Recorder.MicType.Photon:
                                #if PHOTON_MICROPHONE_ENUMERATOR
                                if (Recorder.PhotonMicrophoneEnumerator.IsSupported)
                                {
                                    if (Recorder.PhotonMicrophoneEnumerator.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                    }
                                    else
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        photonDeviceIndex = EditorGUILayout.Popup("Microphone Device", photonDeviceIndex, photonDeviceNames);
                                        recorder.PhotonMicrophoneDeviceId = photonDeviceIDs[photonDeviceIndex];
                                        if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(70)))
                                        {
                                            this.RefreshPhotonMicrophoneDevices();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                else
                                {
                                    recorder.PhotonMicrophoneDeviceId = -1;
                                    EditorGUILayout.HelpBox("PhotonMicrophoneEnumerator Not Supported", MessageType.Error);
                                }
                                #endif
                                #if UNITY_IOS
                                EditorGUILayout.LabelField("iOS Audio Session Parameters", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(useCustomAudioSessionParametersSp, new GUIContent("Use Custom"));
                                if (useCustomAudioSessionParametersSp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(audioSessionParametersCategorySp);
                                    EditorGUILayout.PropertyField(audioSessionParametersModeSp);
                                    EditorGUILayout.PropertyField(audioSessionParametersCategoryOptionsSp, true);
                                }
                                else
                                {
                                    int index = EditorGUILayout.Popup("Preset", audioSessionPresetIndexSp.intValue, iOSAudioSessionPresetsNames);
                                    if (index != audioSessionPresetIndexSp.intValue)
                                    {
                                        audioSessionPresetIndexSp.intValue = index;
                                        AudioSessionParameters parameters = iOSAudioSessionPresetsValues[index];
                                        this.SetEnumIndex(audioSessionParametersCategorySp,
                                            typeof(AudioSessionCategory), parameters.Category);
                                        this.SetEnumIndex(audioSessionParametersModeSp,
                                            typeof(AudioSessionMode), parameters.Mode);
                                        if (parameters.CategoryOptions != null)
                                        {
                                            audioSessionParametersCategoryOptionsSp.ClearArray();
                                            audioSessionParametersCategoryOptionsSp.arraySize =
                                                parameters.CategoryOptions.Length;
                                            if (index == 0)
                                            {
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.DefaultToSpeaker);
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(1), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);
                                            }
                                            else if (index == 1)
                                            {
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);

                                            }
                                        }
                                    }
                                }
                                EditorGUI.indentLevel--;
                                #endif
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case Recorder.InputSourceType.AudioClip:
                        recorder.AudioClip = EditorGUILayout.ObjectField(new GUIContent("Audio Clip", "Source audio clip."), recorder.AudioClip, typeof(AudioClip), false) as AudioClip;
                        recorder.LoopAudioClip =
                            EditorGUILayout.Toggle(new GUIContent("Loop", "Loop playback for audio clip sources."),
                                recorder.LoopAudioClip);
                        break;
                    case Recorder.InputSourceType.Factory:
                        EditorGUILayout.HelpBox("Add a custom InputFactory method in code.", MessageType.Info);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditorGUILayout.LabelField("Voice Activity Detection (VAD)", EditorStyles.boldLabel);
                recorder.VoiceDetection = EditorGUILayout.Toggle(new GUIContent("Detect", "If true, voice detection enabled."), recorder.VoiceDetection);
                if (recorder.VoiceDetection)
                {
                    recorder.VoiceDetectionThreshold =
                        EditorGUILayout.Slider(
                            new GUIContent("Threshold", "Voice detection threshold (0..1, where 1 is full amplitude)."),
                            recorder.VoiceDetectionThreshold, 0f, 1f);
                    recorder.VoiceDetectionDelayMs =
                        EditorGUILayout.IntField(new GUIContent("Delay (ms)", "Keep detected state during this time after signal level dropped below threshold. Default is 500ms"), recorder.VoiceDetectionDelayMs);
                    EditorGUILayout.HelpBox("Do not speak and stay in a silent environment when calibrating.", MessageType.Info);
                    if (recorder.VoiceDetectorCalibrating)
                    {
                        EditorGUILayout.LabelField(string.Format("Calibrating {0} ms", calibrationTime));
                    }
                    else
                    {
                        calibrationTime = EditorGUILayout.IntField("Calibration Time (ms)", calibrationTime);
                        if (recorder.IsRecording && recorder.TransmitEnabled)
                        {
                            if (GUILayout.Button("Calibrate"))
                            {
                                recorder.VoiceDetectorCalibrate(calibrationTime);
                            }
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(this.reactOnSystemChangesSp,
                    new GUIContent("React On System Changes",
                        "If true, recording is restarted when Unity detects Audio Config. changes."));
                EditorGUILayout.PropertyField(this.transmitEnabledSp,
                    new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."));
                EditorGUILayout.PropertyField(this.autoStartSp,
                    new GUIContent("Auto Start", "If true, recording is started when Recorder is initialized."));
                EditorGUILayout.PropertyField(this.encryptSp,
                    new GUIContent("Encrypt", "If true, voice stream is sent encrypted."));
                EditorGUILayout.PropertyField(this.interestGroupSp,
                    new GUIContent("Interest Group", "Target interest group that will receive transmitted audio."));
                if (this.interestGroupSp.intValue == 0)
                {
                    EditorGUILayout.PropertyField(this.debugEchoModeSp,
                        new GUIContent("Debug Echo",
                            "If true, outgoing stream routed back to client via server same way as for remote client's streams."));
                }
                else if (this.debugEchoModeSp.boolValue)
                {
                    Debug.LogWarningFormat("DebugEchoMode disabled because InterestGroup changed to {0}. DebugEchoMode works only with Interest Group 0.", this.interestGroupSp.intValue);
                    this.debugEchoModeSp.boolValue = false;
                }
                EditorGUILayout.PropertyField(this.reliableModeSp, new GUIContent("Reliable Mode",
                        "If true, stream data sent in reliable mode."));

                EditorGUILayout.LabelField("Codec Parameters", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(this.frameDurationSp,
                    new GUIContent("Frame Duration", "Outgoing audio stream encoder delay."));
                EditorGUILayout.PropertyField(this.samplingRateSp,
                    new GUIContent("Sampling Rate", "Outgoing audio stream sampling rate."));
                EditorGUILayout.PropertyField(this.bitrateSp,
                    new GUIContent("Bitrate", "Outgoing audio stream bitrate."));

                EditorGUILayout.LabelField("Audio Source Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(this.sourceTypeSp,
                    new GUIContent("Input Source Type", "Input audio data source type"));
                switch ((Recorder.InputSourceType)this.sourceTypeSp.enumValueIndex)
                {
                    case Recorder.InputSourceType.Microphone:
                        EditorGUILayout.PropertyField(this.microphoneTypeSp, new GUIContent("Microphone Type",
                            "Which microphone API to use when the Source is set to Microphone."));
                        EditorGUILayout.HelpBox("Devices list and current selection is valid in Unity Editor only. In build, you need to set it via code preferably at runtime.", MessageType.Info);
                        switch (recorder.MicrophoneType)
                        {
                            case Recorder.MicType.Unity:
                                if (Microphone.devices.Length == 0)
                                {
                                    EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                }
                                else
                                {
                                    unityMicrophoneDeviceIndex = EditorGUILayout.Popup("Microphone Device", unityMicrophoneDeviceIndex, Microphone.devices);
                                    this.unityMicrophoneDeviceSp.stringValue = Microphone.devices[unityMicrophoneDeviceIndex];
                                    int minFreq, maxFreq;
                                    Microphone.GetDeviceCaps(Microphone.devices[unityMicrophoneDeviceIndex], out minFreq, out maxFreq);
                                    EditorGUILayout.LabelField("Microphone Device Caps", string.Format("{0}..{1} Hz", minFreq, maxFreq));
                                }
                                break;
                            case Recorder.MicType.Photon:
                                #if PHOTON_MICROPHONE_ENUMERATOR
                                if (Recorder.PhotonMicrophoneEnumerator.IsSupported)
                                {
                                    if (Recorder.PhotonMicrophoneEnumerator.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox("No microphone device found", MessageType.Error);
                                    }
                                    else
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        photonDeviceIndex = EditorGUILayout.Popup("Microphone Device", photonDeviceIndex, photonDeviceNames);
                                        this.photonMicrophoneDeviceIdSp.intValue = photonDeviceIDs[photonDeviceIndex];
                                        if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(70)))
                                        {
                                            this.RefreshPhotonMicrophoneDevices();
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                else
                                {
                                    recorder.PhotonMicrophoneDeviceId = -1;
                                    EditorGUILayout.HelpBox("PhotonMicrophoneEnumerator Not Supported", MessageType.Error);
                                }
                                #endif
                                #if UNITY_IOS
                                EditorGUILayout.LabelField("iOS Audio Session Parameters", EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(useCustomAudioSessionParametersSp, new GUIContent("Use Custom"));
                                if (useCustomAudioSessionParametersSp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(audioSessionParametersCategorySp);
                                    EditorGUILayout.PropertyField(audioSessionParametersModeSp);
                                    EditorGUILayout.PropertyField(audioSessionParametersCategoryOptionsSp, true);
                                }
                                else
                                {
                                    int index = EditorGUILayout.Popup("Preset", audioSessionPresetIndexSp.intValue, iOSAudioSessionPresetsNames);
                                    if (index != audioSessionPresetIndexSp.intValue)
                                    {
                                        audioSessionPresetIndexSp.intValue = index;
                                        AudioSessionParameters parameters = iOSAudioSessionPresetsValues[index];
                                        this.SetEnumIndex(audioSessionParametersCategorySp,
                                            typeof(AudioSessionCategory), parameters.Category);
                                        this.SetEnumIndex(audioSessionParametersModeSp,
                                            typeof(AudioSessionMode), parameters.Mode);
                                        if (parameters.CategoryOptions != null)
                                        {
                                            audioSessionParametersCategoryOptionsSp.ClearArray();
                                            audioSessionParametersCategoryOptionsSp.arraySize =
                                                parameters.CategoryOptions.Length;
                                            if (index == 0)
                                            {
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.DefaultToSpeaker);
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(1), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);
                                            }
                                            else if (index == 1)
                                            {
                                                this.SetEnumIndex(audioSessionParametersCategoryOptionsSp
                                                    .GetArrayElementAtIndex(0), typeof(AudioSessionCategoryOption), AudioSessionCategoryOption.AllowBluetooth);

                                            }
                                        }
                                    }
                                }
                                EditorGUI.indentLevel--;
                                #endif
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case Recorder.InputSourceType.AudioClip:
                        EditorGUILayout.PropertyField(this.audioClipSp,
                            new GUIContent("Audio Clip", "Source audio clip."));
                        EditorGUILayout.PropertyField(this.loopAudioClipSp,
                            new GUIContent("Loop", "Loop playback for audio clip sources."));
                        break;
                    case Recorder.InputSourceType.Factory:
                        EditorGUILayout.HelpBox("Add a custom InputFactory method in code.", MessageType.Info);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                EditorGUILayout.LabelField("Voice Activity Detection (VAD)", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(this.voiceDetectionSp,
                    new GUIContent("Detect", "If true, voice detection enabled."));
                if (this.voiceDetectionSp.boolValue)
                {
                    this.voiceDetectionThresholdSp.floatValue = EditorGUILayout.Slider(
                            new GUIContent("Threshold", "Voice detection threshold (0..1, where 1 is full amplitude)."),
                            this.voiceDetectionThresholdSp.floatValue, 0f, 1f);
                    this.voiceDetectionDelayMsSp.intValue =
                        EditorGUILayout.IntField(new GUIContent("Delay (ms)", "Keep detected state during this time after signal level dropped below threshold. Default is 500ms"), this.voiceDetectionDelayMsSp.intValue);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnAudioConfigChanged(bool deviceWasChanged)
        {
            if (deviceWasChanged)
            {
                this.RefreshMicrophones();
            }
        }

        private void RefreshMicrophones()
        {
            if (Microphone.devices.Length == 0)
            {
                recorder.UnityMicrophoneDevice = null;
                unityMicrophoneDeviceIndex = 0;
            }
            else
            {
                unityMicrophoneDeviceIndex = Mathf.Clamp(ArrayUtility.IndexOf(Microphone.devices, recorder.UnityMicrophoneDevice), 0, Microphone.devices.Length - 1);
            }
            #if PHOTON_MICROPHONE_ENUMERATOR
            this.RefreshPhotonMicrophoneDevices();
            #endif
        }

        #if PHOTON_MICROPHONE_ENUMERATOR
        private void RefreshPhotonMicrophoneDevices()
        {
            if (Recorder.PhotonMicrophoneEnumerator.IsSupported)
            {
                Recorder.PhotonMicrophoneEnumerator.Refresh();
                if (Recorder.PhotonMicrophoneEnumerator.Count == 0)
                {
                    recorder.PhotonMicrophoneDeviceId = -1;
                    photonDeviceNames = null;
                    photonDeviceIDs = null;
                    photonDeviceIndex = 0;
                }
                else
                {
                    photonDeviceNames = new string[Recorder.PhotonMicrophoneEnumerator.Count];
                    photonDeviceIDs = new int[Recorder.PhotonMicrophoneEnumerator.Count];
                    List<string> tempNames = new List<string>(photonDeviceNames.Length);
                    for (int i = 0; i < Recorder.PhotonMicrophoneEnumerator.Count; i++)
                    {
                        photonDeviceIDs[i] = Recorder.PhotonMicrophoneEnumerator.IDAtIndex(i);
                        string micName = Recorder.PhotonMicrophoneEnumerator.NameAtIndex(i);
                        int count = 0;
                        for (int j = 0; j < tempNames.Count; j++)
                        {
                            if (tempNames[j].StartsWith(micName))
                            {
                                count++;
                            }
                        }
                        tempNames.Add(string.Format("{0} - {1}{2}", photonDeviceIDs[i], micName, count == 0 ? string.Empty : string.Format(" {0}", count)));
                    }
                    photonDeviceNames = tempNames.ToArray();
                    photonDeviceIndex = Mathf.Clamp(Array.IndexOf(photonDeviceIDs,
                            recorder.PhotonMicrophoneDeviceId), 0, Recorder.PhotonMicrophoneEnumerator.Count - 1);
                    recorder.PhotonMicrophoneDeviceId = photonDeviceIDs[photonDeviceIndex];
                }
            }
            else
            {
                recorder.PhotonMicrophoneDeviceId = -1;
            }
            
        }
        #endif

        private void SetEnumIndex(SerializedProperty property, Type enumType, object enumValue)
        {
            string enumName = Enum.GetName(enumType, enumValue);
            int index = Array.IndexOf(property.enumNames, enumName);
            if (index >= 0)
            {
                property.enumValueIndex = index;
            }
        }
    }
}