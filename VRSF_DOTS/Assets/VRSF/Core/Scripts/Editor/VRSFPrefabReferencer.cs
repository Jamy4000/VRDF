using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Core.Utils
{
    //[CreateAssetMenu()]
    public class VRSFPrefabReferencer : ScriptableSingleton<VRSFPrefabReferencer>
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
            Debug.LogErrorFormat("<b>[VRSF] :</b> An error has occured while loading the prefab {0}. Please add an issue on Github or contact me via email (arnaudbriche1994@gmail.com) so I can fix that ASAP !", name);
            return null;
        }
    }
}