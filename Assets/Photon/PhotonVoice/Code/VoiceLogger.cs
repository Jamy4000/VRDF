// ----------------------------------------------------------------------------
// <copyright file="VoiceLogger.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Logger for voice components.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Voice.Unity
{
    using ExitGames.Client.Photon;
    using UnityEngine;

    public class VoiceLogger : Voice.ILogger
    {
        public VoiceLogger(Object context, string tag, DebugLevel level = DebugLevel.ERROR)
        {
            this.context = context;
            this.Tag = tag;
            this.LogLevel = level;
        }

        public VoiceLogger(string tag, DebugLevel level = DebugLevel.ERROR)
        {
            this.Tag = tag;
            this.LogLevel = level;
        }

        public string Tag { get; set; }

        public DebugLevel LogLevel { get; set; }

        public bool IsErrorEnabled
        {
            get { return this.LogLevel >= DebugLevel.ERROR; }
        }

        public bool IsWarningEnabled
        {
            get { return this.LogLevel >= DebugLevel.WARNING; }
        }

        public bool IsInfoEnabled
        {
            get { return this.LogLevel >= DebugLevel.INFO; }
        }

        public bool IsDebugEnabled { get { return this.LogLevel == DebugLevel.ALL; } }

        private Object context;
        
        #region ILogger

        public void LogError(string fmt, params object[] args)
        {
            if (!this.IsErrorEnabled) return;
            fmt = string.Format("[{0}] {1}", Tag, fmt);
            if (context == null)
            {
                Debug.LogErrorFormat(fmt, args);
            }
            else
            {
                Debug.LogErrorFormat(context, fmt, args);
            }
        }

        public void LogWarning(string fmt, params object[] args)
        {
            if (!this.IsWarningEnabled) return;
            fmt = string.Format("[{0}] {1}", Tag, fmt);
            if (context == null)
            {
                Debug.LogWarningFormat(fmt, args);
            }
            else
            {
                Debug.LogWarningFormat(context, fmt, args);
            }
        }

        public void LogInfo(string fmt, params object[] args)
        {
            if (!this.IsInfoEnabled) return;
            fmt = string.Format("[{0}] {1}", Tag, fmt);
            if (context == null)
            {
                Debug.LogFormat(fmt, args);
            }
            else
            {
                Debug.LogFormat(context, fmt, args);
            }
        }

        public void LogDebug(string fmt, params object[] args)
        {
            if (!this.IsDebugEnabled) return;
            LogInfo(fmt, args);
        }

        #endregion

        #if UNITY_EDITOR
        public static void ExposeLogLevel(UnityEditor.SerializedObject obj, ILoggable loggable)
        {
            UnityEditor.SerializedProperty logLevelSp = obj.FindProperty("logLevel");
            UnityEditor.EditorGUI.BeginChangeCheck();
            UnityEditor.EditorGUILayout.PropertyField(logLevelSp, new GUIContent("Log Level", "Logging level for this Photon Voice component."));
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                if (logLevelSp.enumValueIndex == 4)
                {
                    loggable.LogLevel = DebugLevel.ALL;
                }
                else
                {
                    loggable.LogLevel = (DebugLevel)logLevelSp.enumValueIndex;
                }
                obj.ApplyModifiedProperties();
            }
        }
        #endif
    }
}
