using System.Collections.Generic;
using UnityEngine.Events;

namespace VRSF.Core.Utils
{
    public static class UnityEventExtension
    {
        public static Dictionary<UnityEvent, List<UnityAction>> _nonPersistentListeners = new Dictionary<UnityEvent, List<UnityAction>>();

        public static void AddListenerExtend(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.AddListener(unityAction);

            if (_nonPersistentListeners.ContainsKey(unityEvent))
                _nonPersistentListeners[unityEvent].Add(unityAction);
            else
                _nonPersistentListeners.Add(unityEvent, new List<UnityAction> { unityAction });
        }

        public static void RemoveListenerExtend(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveListener(unityAction);

            if (_nonPersistentListeners.ContainsKey(unityEvent))
                _nonPersistentListeners[unityEvent].Remove(unityAction);
        }

        public static void RemoveAllListenersExtend(this UnityEvent unityEvent)
        {
            unityEvent.RemoveAllListeners();

            if (_nonPersistentListeners.ContainsKey(unityEvent))
                _nonPersistentListeners[unityEvent].Clear();
        }

        public static int GetNonPersistentListenersCount(this UnityEvent unityEvent)
        {
            if (_nonPersistentListeners.ContainsKey(unityEvent))
                return _nonPersistentListeners[unityEvent].Count;

            //else
            System.Console.WriteLine("No non persistent listener was found for the following event : " + unityEvent.ToString());
            return -1;
        }


        public static List<UnityAction> GetNonPersistentListeners(this UnityEvent unityEvent)
        {
            if (_nonPersistentListeners.ContainsKey(unityEvent))
                return _nonPersistentListeners[unityEvent];

            //else
            System.Console.WriteLine("No non persistent listeners were found for the following event : " + unityEvent.ToString());
            return null;
        }
    }
}