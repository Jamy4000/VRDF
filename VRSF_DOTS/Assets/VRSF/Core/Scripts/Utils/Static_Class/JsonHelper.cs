using System;
using System.IO;
using UnityEngine;

namespace VRSF.Core.Utils
{
    /// <summary>
    /// Helper class to translate an object to JSON file and vice versa
    /// WARNING : The JSON needs to be created using the ToJson method before you can read it with FromJson
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Feed the Json as text and return a list of objects contained in the JSON
        /// </summary>
        /// <typeparam name="T">The type to which the json file is gonna be translated</typeparam>
        /// <param name="json">The Json file as a string</param>
        /// <returns>The new array of object of type T</returns>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        /// <summary>
        /// Translate a list of Item of type T to a JSON-ready string
        /// </summary>
        /// <typeparam name="T">The type you want to translate to Json</typeparam>
        /// <param name="array">The array of T you want to add in the JSON</param>
        /// <returns>The newly created JSON-ready string.</returns>
        public static string ToJson<T>(T[] array, bool prettyPrint = true)
        {
            Wrapper<T> wrapper = new Wrapper<T>
            {
                Items = array
            };
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        /// <summary>
        /// Generate a JSON file from the json-ready string and the path given
        /// </summary>
        /// <param name="newJSON">The newly created JSON-ready string</param>
        /// <param name="path">The path where we save the JSON file. StreamingAssets is strongly recommended.</param>
        public static void GenerateNewJSON(string newJSON, string path)
        {
            // if the file doesn't exists, create file
            if (!File.Exists(path))
            {
                Debug.Log("File doesn't exist");
                var file = File.Create(path);
                file.Dispose();
            }

            // Empty the file
            File.WriteAllText(path, string.Empty);

            // Rewrite the file
            File.WriteAllText(path, newJSON);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}