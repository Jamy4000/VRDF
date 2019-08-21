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
        
        private void OnEnable()
        {
            processor = target as WebRtcAudioDsp;
            recorder = processor.GetComponent<Recorder>();
        }

        public override void OnInspectorGUI()
        {
            if (processor.VAD && recorder.VoiceDetection)
            {
                EditorGUILayout.HelpBox("You have enabled VAD here and in the associated Recorder. Please use only one Voice Detection algorithm.", MessageType.Warning);
            }

            if ((processor.AEC || processor.AECMobile) && recorder.MicrophoneType == Recorder.MicType.Photon)
            {
                EditorGUILayout.HelpBox("You have enabled AEC here and are using a Photon Mic as input on the Recorder, which might add its own echo cancellation. Please use only one AEC algorithm.", MessageType.Warning);
            }

            VoiceLogger.ExposeLogLevel(this.serializedObject, processor);
            EditorGUI.BeginChangeCheck();
            processor.Bypass = EditorGUILayout.Toggle(new GUIContent("Bypass", "Bypass WebRTC Audio DSP"), processor.Bypass);
            processor.AEC = EditorGUILayout.Toggle(new GUIContent("AEC", "Acoustic Echo Cancellation"), processor.AEC);
            processor.AECMobile = EditorGUILayout.Toggle(new GUIContent("AEC Mobile", "Acoustic Echo Cancellation Mobile"), processor.AECMobile);
            if (processor.AEC || processor.AECMobile)
            {
                processor.ReverseStreamDelayMs = EditorGUILayout.IntField(new GUIContent("ReverseStreamDelayMs", "Reverse stream delay (hint for AEC) in Millieconds"), processor.ReverseStreamDelayMs);
            }
            if (processor.AECMobile)
            {
                processor.AECMobileComfortNoise = EditorGUILayout.Toggle(new GUIContent("AEC Mobile Comfort Noise", "Acoustic Echo Cancellation Mobile Comfort Noise"), processor.AECMobileComfortNoise);
            }
            processor.AGC = EditorGUILayout.Toggle(new GUIContent("AGC", "Automatic Gain Control"), processor.AGC);
            processor.VAD = EditorGUILayout.Toggle(new GUIContent("VAD", "Voice Activity Detection"), processor.VAD);
            processor.HighPass = EditorGUILayout.Toggle(new GUIContent("HighPass", "High Pass Filter"), processor.HighPass);
            processor.NoiseSuppression = EditorGUILayout.Toggle(new GUIContent("NoiseSuppression", "Noise Suppression"), processor.NoiseSuppression);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
