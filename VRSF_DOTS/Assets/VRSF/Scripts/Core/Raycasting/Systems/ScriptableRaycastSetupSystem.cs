using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Interactions;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Setup the references for Scriptable Variables depending on the RayOrigin
    /// </summary>
    public class ScriptableRaycastSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComp;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += SetupVariables;
            base.OnCreateManager();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= SetupVariables;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupVariables(OnSetupVRReady setupVRReady)
        {
            var interactionsContainer = InteractionVariableContainer.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                e.RaycastComp._VRCamera = VRSF_Components.VRCamera.GetComponent<Camera>();

                switch (e.RaycastComp.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:

                        e.RaycastComp.RayVar = interactionsContainer.LeftRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.LeftHit;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.LeftController.transform;
                        break;

                    case ERayOrigin.RIGHT_HAND:
                        e.RaycastComp.RayVar = interactionsContainer.RightRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.RightHit;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.RightController.transform;
                        break;

                    case ERayOrigin.CAMERA:
                        e.RaycastComp.RayVar = interactionsContainer.GazeRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.GazeHit;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.VRCamera.transform;
                        break;
                }

                e.RaycastComp.IsSetup = true;
            }
        }
        #endregion PRIVATES_METHODS
    }
}