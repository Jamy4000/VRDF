using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Core.Utils
{
    //[CreateAssetMenu()]
    public class VRDFPrefabReferencer : ScriptableSingleton<VRDFPrefabReferencer>
    {
        [SerializeField] public List<GameObject> PrefabsReferences;

        public static GameObject GetPrefab(string name)
        {
            if (Instance != null)
            {
                foreach (var prefab in Instance.PrefabsReferences)
                {
                    if (prefab != null && prefab.name == name)
                        return prefab;
                }
            }
            Debug.LogErrorFormat("<b>[VRDF] :</b> An error has occured while loading the prefab {0}. Please add an issue on Github or contact me via email (arnaudbriche1994@gmail.com) so I can fix that ASAP !", name);
            return null;
        }
    }
}