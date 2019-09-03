// Credits are going to Arcturus Studio (http://arcturus.studio), that provide this script in one of its repository : https://github.com/Arcturus-Studio/TOUnityUtilities/blob/master/Assets/TOUtilities/ScriptableSingleton.cs
// It has been refactored to avoid some unnecessary methods and to add some comments

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace VRSF.Core.Utils
{
    /// <summary>
    ///  Utility class for handling singleton ScriptableObjects for data management
    /// </summary>
    /// <typeparam name="T">The type of ScriptableObject to set as Singleton</typeparam>
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _cachedInstance;
        
        /// <summary>
        /// The name of the scriptable file
        /// </summary>
        private static string _fileName
        {
            get
            {
                return typeof(T).Name;
            }
        }
        
        /// <summary>
        /// The path of the Asset
        /// </summary>
        private static string _vrsfResources
        {
            get
            {
                return "Assets/VRSF/Core/Resources";
            }
        }

        /// <summary>
        /// Return an Instance of the ScriptableSingleton
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_cachedInstance == null)
                {
                    _cachedInstance = Resources.Load<T>(_fileName) as T;
                }
#if UNITY_EDITOR
                if (_cachedInstance == null)
                {
                    _cachedInstance = CreateAndSave();
                }
#endif
                if (_cachedInstance == null)
                {
                    Debug.LogWarning("No instance of " + _fileName + " found, using default values");
                    _cachedInstance = ScriptableObject.CreateInstance<T>();
                }

                return _cachedInstance;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create and save a ScriptableSingleton
        /// </summary>
        /// <returns>The new ScritpableSingleton created</returns>
        protected static T CreateAndSave()
        {
            T instance = ScriptableObject.CreateInstance<T>();

            //Saving during Awake() will crash Unity, delay saving until next editor frame
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall += () => SaveAsset(instance);
            }
            else
            {
                SaveAsset(instance);
            }
            return instance;
        }

        /// <summary>
        /// Save a ScriptableSingleton as an Asset
        /// </summary>
        /// <param name="obj">The type of Scriptable Object to save</param>
        private static void SaveAsset(T obj)
        {
            if (!Directory.Exists(_vrsfResources))
                Directory.CreateDirectory(_vrsfResources);

            string filePath = Path.Combine(_vrsfResources, _fileName + ".asset");
            AssetDatabase.CreateAsset(obj, filePath);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}