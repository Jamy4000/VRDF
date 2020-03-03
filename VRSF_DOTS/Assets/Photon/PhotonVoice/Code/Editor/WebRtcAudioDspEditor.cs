using UnityEngine;

namespace Photon.Voice.Unity.Editor
{
    using UnityEditor;
    using Unity;

    [CustomEditor(typeof(WebRtcAudioDsp))]
    public class WebRtcAudioDspEditor : Editor
    {
        private WebRtcAudioDsp processor;
        private Recorder recorder;

        private SerializedProperty aecSp;
        private SerializedProperty aecMobileSp;
        private SerializedProperty aecMobileComfortNoiseSp;
        private SerializedProperty agcSp;
        private SerializedProperty vadSp;
        private SerializedProperty highPassSp;
        private SerializedProperty bypassSp;
        private SerializedProperty noiseSuppressionSp;
        private SerializedProperty reverseStreamDelayMsSp;

        private void OnEnable()
        {
            processor = target as WebRtcAudioDsp;
            recorder = processor.GetComponent<Recorder>();
            this.aecSp = this.serializedObject.FindProperty("aec");
            this.aecMobileSp = this.serializedObject.FindProperty("aecMobile");
            this.aecMobileComfortNoiseSp = this.serializedObject.FindProperty("aecMobileComfortNoise");
            this.agcSp = this.serializedObject.FindProperty("agc");
            this.vadSp = this.serializedObject.FindProperty("vad");
            this.highPassSp = this.serializedObject.FindProperty("highPass");
            this.bypassSp = this.serializedObject.FindProperty("bypass");
            this.noiseSuppressionSp = this.serializedObject.FindProperty("noiseSuppression");
            this.reverseStreamDelayMsSp = this.serializedObject.FindProperty("reverseStreamDelayMs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            if (!processor.enabled)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp is disabled and will not be used.", MessageType.Warning);
            }
            if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone)
            {
                EditorGUILayout.HelpBox("WebRtcAudioDsp should be used with Microphone as Recorder Input Source Type.", MessageType.Warning);
            }
            VoiceLogger.ExposeLogLevel(this.serializedObject, processor);
            bool bypassed = false;
            EditorGUI.BeginChangeCheck();
            if (Application.isPlaying)
            {
                processor.Bypass = EditorGUILayout.Toggle(new GUIContent("Bypass", "Bypass WebRTC Audio DSP"), processor.Bypass);
                bypassed = processor.Bypass;
            }
            else
            {
                EditorGUILayout.PropertyField(this.bypassSp, new GUIContent("Bypass", "Bypass WebRTC Audio DSP"));
                bypassed = this.bypassSp.boolValue;
            }

            if (!bypassed)
            {
                if (Application.isPlaying)
                {
                    processor.AEC = EditorGUILayout.Toggle(new GUIContent("AEC", "Acoustic Echo Cancellation"), processor.AEC);
                    processor.AECMobile = EditorGUILayout.Toggle(new GUIContent("AEC Mobile", "Acoustic Echo Cancellation Mobile"), processor.AECMobile);
                    if (processor.AEC || processor.AECMobile)
                    {
                        if (recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                        }
                        processor.ReverseStreamDelayMs = EditorGUILayout.IntField(new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"), processor.ReverseStreamDelayMs);
                    }
                    if (processor.AECMobile)
                    {
                        processor.AECMobileComfortNoise = EditorGUILayout.Toggle(new GUIContent("AEC Mobile Comfort Noise", "Acoustic Echo Cancellation Mobile Comfort Noise"), processor.AECMobileComfortNoise);
                    }
                    processor.AGC = EditorGUILayout.Toggle(new GUIContent("AGC", "Automatic Gain Control"), processor.AGC);
                    if (processor.VAD && recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    processor.VAD = EditorGUILayout.Toggle(new GUIContent("VAD", "Voice Activity Detection"), processor.VAD);
                    processor.HighPass = EditorGUILayout.Toggle(new GUIContent("HighPass", "High Pass Filter"), processor.HighPass);
                    processor.NoiseSuppression = EditorGUILayout.Toggle(new GUIContent("NoiseSuppression", "Noise Suppression"), processor.NoiseSuppression);
                }
                else
                {
                    bool aec = this.aecSp.boolValue;
                    bool aecMobile = this.aecMobileSp.boolValue;
                    aec = EditorGUILayout.Toggle(new GUIContent("AEC", "Acoustic Echo Cancellation"), aec);
                    if (aec && aecMobile)
                    {
                        aecMobile = false;
                    }                    
                    aecMobile = EditorGUILayout.Toggle(new GUIContent("AEC Mobile", "Acoustic Echo Cancellation Mobile"), aecMobile);
                    if (aec && aecMobile)
                    {
                        aec = false;
                    }
                    this.aecSp.boolValue = aec;
                    this.aecMobileSp.boolValue = aecMobile;
                    if (this.aecSp.boolValue || this.aecMobileSp.boolValue)
                    {
                        if (recorder.MicrophoneType == Recorder.MicType.Photon)
                        {
                            EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
                        }
                        EditorGUILayout.PropertyField(this.reverseStreamDelayMsSp,
                            new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Milliseconds"));
                    }
                    if (this.aecMobileSp.boolValue)
                    {
                        EditorGUILayout.PropertyField(this.aecMobileComfortNoiseSp, new GUIContent("AEC Mobile Comfort Noise", "Acoustic Echo Cancellation Mobile Comfort Noise"));
                    }
                    EditorGUILayout.PropertyField(this.agcSp, new GUIContent("AGC", "Automatic Gain Control"));
                    if (this.vadSp.boolValue && recorder.VoiceDetection)
                    {
                        EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(this.vadSp, new GUIContent("VAD", "Voice Activity Detection"));
                    EditorGUILayout.PropertyField(this.highPassSp, new GUIContent("HighPass", "High Pass Filter"));
                    EditorGUILayout.PropertyField(this.noiseSuppressionSp, new GUIContent("NoiseSuppression", "Noise Suppression"));
                }
            }
                
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
