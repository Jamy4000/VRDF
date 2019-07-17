using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Core.Utils
{
    //[CreateAssetMenu()]
    public class VRSFPrefabReferencer : UnityEditor.ScriptableSingleton<VRSFPrefabReferencer>
    {
        public List<string> PrefabsNames = new List<string>();

        public List<GameObject> PrefabsReferences = new List<GameObject>();

        public Dictionary<string, GameObject> PrefabsDictionary = new Dictionary<string, GameObject>();

        private void OnValidate()
        {
            PrefabsDictionary = new Dictionary<string, GameObject>();
            for (int i = 0; i < PrefabsNames.Count; i++)
            {
                if (PrefabsReferences.Count >= i)
                    PrefabsDictionary.Add(PrefabsNames[i], PrefabsReferences[i]);
            }
        }
    }
}