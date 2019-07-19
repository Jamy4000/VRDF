using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Parabolic Pointers Feature
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolicPointerInitSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += InitValues;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= InitValues;
        }

        /// <summary>
        /// Initialize the values for the Parabolic Pointer feature.
        /// </summary>
        /// <param name="e">The entity to check in the scene</param>
        private void InitValues(OnSetupVRReady setupVRReady)
        {
            //foreach (var e in GetEntities<Filter>())
            //{
            //    e.PointerObjects._ControllerPointer = e.BACGeneral.ButtonHand == Core.Controllers.EHand.LEFT ?
            //        VRSF_Components.LeftController.GetComponentInChildren<LineRenderer>() :
            //        VRSF_Components.RightController.GetComponentInChildren<LineRenderer>();

            //    if (e.PointerObjects._selectionPadPrefab != null)
            //    {
            //        e.PointerObjects._selectionPadObject = GameObject.Instantiate(e.PointerObjects._selectionPadPrefab);
            //        e.PointerObjects._selectionPadObject.transform.SetParent(e.PointerObjects.transform);
            //        e.PointerObjects._selectionPadObject.SetActive(false);
            //    }

            //    if (e.PointerObjects._invalidPadPrefab != null)
            //    {
            //        e.PointerObjects._invalidPadObject = GameObject.Instantiate(e.PointerObjects._invalidPadPrefab);
            //        e.PointerObjects._invalidPadObject.transform.SetParent(e.PointerObjects.transform);
            //        e.PointerObjects._invalidPadObject.SetActive(false);
            //    }
            //}
        }
    }
}