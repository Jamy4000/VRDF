using System.Collections;
using UnityEngine;

namespace VRDF.Core
{
    /// <summary>
    /// Destroy the gameobject attached to this object one frame after SetupVR is ready
    /// </summary>
    public class SetupVRDestroyer : MonoBehaviour
    {
        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(DestroyThisObject);
        }

        private void OnDestroy()
        {
            OnSetupVRReady.UnregisterSetupVRCallback(DestroyThisObject);
        }

        private void DestroyThisObject(OnSetupVRReady _)
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