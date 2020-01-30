using System;
using System.Collections;
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
        /// Generate a JSON file from the json-ready string at the given path
        /// </summary>
        /// <param name="newJSON">The newly created JSON-ready string</param>
        /// <param name="pathToFile">The path where we save the JSON file</param>
        public static void GenerateNewJSON(string newJSON, string pathToFile, MonoBehaviour coroutineStarter)
        {
            // Get the path to the directory containing the file
            string filesDirectoryPath = Path.GetDirectoryName(pathToFile);

            // if the file doesn't exists, create file
            if (!Directory.Exists(filesDirectoryPath))
                Directory.CreateDirectory(filesDirectoryPath);

            // if the file doesn't exists, create file
            if (!File.Exists(pathToFile))
                CreateFile();

            // We start the coroutine to check if the file is correctly created
            coroutineStarter.StartCoroutine(WaitUntilFileIsCreated());

            IEnumerator WaitUntilFileIsCreated()
            {
                while (!File.Exists(pathToFile))
                {
                    Debug.Log("File still not existing, waiting for next frame.");
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("File found at path " + pathToFile);

                coroutineStarter.StartCoroutine(WriteText());
            }

            void CreateFile()
            {
                var file = File.Create(pathToFile);
                file.Dispose();
            }

            IEnumerator WriteText()
            {
                // Empty the file
                File.WriteAllText(pathToFile, string.Empty);

                yield return new WaitForEndOfFrame();

                // Rewrite the file
                File.WriteAllText(pathToFile, newJSON);
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}