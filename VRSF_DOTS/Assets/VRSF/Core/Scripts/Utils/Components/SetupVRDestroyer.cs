using System.Collections;
using UnityEngine;

namespace VRSF.Core
{
    /// <summary>
    /// Destroy the gameobject attached to this object one frame after SetupVR is ready
    /// </summary>
    public class SetupVRDestroyer : MonoBehaviour
    {
        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(DestroyThisObject);
        }

        private void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(DestroyThisObject))
                OnSetupVRReady.Listeners -= DestroyThisObject;
        }

        private void DestroyThisObject(OnSetupVRReady info)
        {
            StartCoroutine(Destroying());

            IEnumerator Destroying()
            {
                yield return new WaitForEndOfFrame();
                Destroy(gameObject);
            }
        }
    }
}