using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Core.Utils
{
    [CreateAssetMenu()]
    public class VRSFPrefabReferencer : ScriptableSingleton<VRSFPrefabReferencer>
    {
        [SerializeField] public List<GameObject> PrefabsReferences;

        public static GameObject GetPrefab(string name)
        {
            Debug.Log("Instance " + Instance.PrefabsReferences);
            foreach (var prefab in Instance.PrefabsReferences)
            {
                Debug.Log("prefab.name : " + prefab.name);
                Debug.Log("name : " + name);
                if (prefab.name == name)
                    return prefab;
            }
            Debug.LogErrorFormat("<b>[VRSF] :</b> An error has occured while loading the prefab {0}. Please add an issue on Github or contact me via email (arnaudbriche1994@gmail.com) so I can fix that ASAP !", name);
            return null;
        }
    }
}