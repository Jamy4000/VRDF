namespace Photon.Voice.Unity.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Unity;

    [CustomEditor(typeof(Speaker))]
    public class SpeakerEditor : Editor
    {
        private Speaker speaker;

        private SerializedProperty playDelayMsSp;

        #region AnimationCurve

        private AudioSource audioSource;
        private FFTWindow window = FFTWindow.Hanning;
        private float[] samples = new float[512];
        private AnimationCurve curve;

        private void DrawAnimationCurve()
        {
            audioSource.GetSpectrumData(samples, 0, window);
            curve = new AnimationCurve();
            for (var i = 0; i < samples.Length; i++)
            {
                curve.AddKey(1.0f / samples.Length * i, samples[i] * 100);
            }
            EditorGUILayout.CurveField(curve, Color.green, new Rect(0, 0, 1.0f, 0.1f), GUILayout.Height(64));
        }

        #endregion


        private void OnEnable()
        {
            speaker = target as Speaker;
            audioSource = speaker.GetComponent<AudioSource>();
            playDelayMsSp = serializedObject.FindProperty("PlayDelayMs");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            //            serializedObject.UpdateIfRequiredOrScript();
            VoiceLogger.ExposeLogLevel(serializedObject, speaker);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(playDelayMsSp, new GUIContent("Playback Delay (ms)", "Remote audio stream playback delay to compensate packets latency variations. Try 100 - 200 if sound is choppy. Default is 200ms"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (speaker.IsPlaying)
            {
                EditorGUILayout.LabelField(string.Format("Current Buffer Lag: {0}", speaker.Lag));
                DrawAnimationCurve();
            }
        }
    }
}