
using UnityEngine;
using System.Collections.Generic;

namespace ScriptableFramework.RuntimeSet
{
    /// <summary>
    /// Generic dictionnary set
    /// All key entry must be UNIQUE
    /// </summary>
    /// <typeparam name="K">Type of the key</typeparam>
    /// <typeparam name="V">Type of the value</typeparam>
    public class ScriptableDictionnary<K, V> : ScriptableObject
    {
        /// <summary>
        /// Dictionnary holding the key/value pair 
        /// </summary>
        public Dictionary<K, V> Items { get; set; }

        [Header("List of Keys")]
        [Tooltip("They must be placed in the same order as the values, ie. Keys[0] = Values[0]")]
        [SerializeField]
        List<K> KeyList = new List<K>();

        [Header("List of Values")]
        [Tooltip("They must be placed in the same order as the keys, ie. Keys[0] = Values[0")]
        [SerializeField]
        List<V> ValueList = new List<V>();

        private void OnEnable()
        {
            Items = new Dictionary<K, V>();
            for(int i = 0; i < KeyList.Count; i++)
            {
                Items.Add(KeyList[i], ValueList[i]);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < KeyList.Count; i++)
            {
                Items.Remove(KeyList[i]);
            }
            Items = null;
        }

        /// <summary>
        /// Add a new entry to the dictionnary
        /// </summary>
        /// <param name="key">New key to be added to the dictionnary</param>
        /// <param name="value">Entry's value to be added to the dictionnary</param>
        public void Add(K key, V value)
        {
            if (!Items.ContainsKey(key))
            {
                Items.Add(key, value);
            }
        }

        /// <summary>
        /// Remove an entry from the dictionnary 
        /// </summary>
        /// <param name="key">Key of the entry to be removed</param>
        public void Remove(K key)
        {
            if (Items.ContainsKey(key))
            {
                Items.Remove(key);
            }
        }

        /// <summary>
        /// Access a value based on its key
        /// </summary>
        /// <param name="key">Key to acces the value</param>
        /// <returns>The T value if it exists in the dictionnary or null if it does not exist</returns>
        public V Get(K key)
        {
            if (Items.ContainsKey(key))
            {
                return Items[key];
            }
            else
            {
                return default(V);
            }
        }
    }
}