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
            // serializedObject.UpdateIfRequiredOrScript()
            if (Application.isPlaying)
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
                    EditorGUILayout.HelpBox("Recorder requires initialization. Call Recorder.Init(VoiceClient, Object).", MessageType.Warning);
                }
            }
            VoiceLogger.ExposeLogLevel(serializedObject, recorder);
            EditorGUI.BeginChangeCheck();
            recorder.ReactOnSystemChanges = EditorGUILayout.Toggle(new GUIContent("React On System Changes", "If true, recording is restarted when Unity detects Audio Config. changes."), recorder.ReactOnSystemChanges);
            recorder.AutoStart = EditorGUILayout.Toggle(new GUIContent("Auto Start", "If true, recording is started when Recorder is initialized."), recorder.AutoStart);
            recorder.TransmitEnabled = EditorGUILayout.Toggle(new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."), recorder.TransmitEnabled);
            if (recorder.IsInitialized)
            {
                recorder.IsRecording = EditorGUILayout.Toggle(new GUIContent("IsRecording", "If true, audio recording is on."), recorder.IsRecording);
            }
            if (recorder.IsRecording)
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
                    if (recorder.IsRecording)
                    {
                        if (GUILayout.Button("Calibrate"))
                        {
                            recorder.VoiceDetectorCalibrate(calibrationTime);
                        }
                    }
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