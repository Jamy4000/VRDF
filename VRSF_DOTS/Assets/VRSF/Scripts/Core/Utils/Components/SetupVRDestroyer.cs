using System;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Utils
{
    /// <summary>
    /// Destroy the gameobject attached to this object when SertupVR is ready
    /// </summary>
    public class SetupVRDestroyer : MonoBehaviour
    {
        private void Awake()
        {
            OnSetupVRReady.Listeners += DestroyThisObject;
        }
        private void OnDestroy()
        {
            OnSetupVRReady.Listeners -= DestroyThisObject;
        }

        private void DestroyThisObject(OnSetupVRReady info)
        {
            Destroy(gameObject);
        }

    }
}