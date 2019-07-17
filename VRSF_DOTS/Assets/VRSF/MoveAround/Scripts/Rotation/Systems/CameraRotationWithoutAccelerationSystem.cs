//using UnityEngine;
//using VRSF.Core.Inputs;
//using VRSF.Core.SetupVR;
//using VRSF.Core.Utils.ButtonActionChoser;
//using VRSF.Core.Raycast;
//using VRSF.Core.Utils;

//namespace VRSF.MoveAround.Rotate
//{
//    public class CameraRotationWithoutAccelerationSystem : BACListenersSetupSystem
//    {
//        struct Filter
//        {
//            public UserRotationAuthoring RotationComp;
//            public BACGeneralComponent BACGeneral;
//            public BACCalculationsComponent BACCalculations;
//        }

//        #region ComponentSystem_Methods
//        protected override void OnCreateManager()
//        {
//            base.OnCreateManager();
//            OnSetupVRReady.Listeners += Init;
//        }

//        protected override void OnDestroyManager()
//        {
//            base.OnDestroyManager();
//            foreach (var e in GetEntities<Filter>())
//            {
//                RemoveListeners(e);
//            }
//            OnSetupVRReady.Listeners -= Init;
//        }
//        #endregion


//        #region PUBLIC_METHODS
//        public override void SetupListenersResponses(object entity)
//        {
//            var e = (Filter)entity;
//            if (e.RotationComp.ActionWithoutAcceleration == null)
//            {
//                e.RotationComp.ActionWithoutAcceleration = delegate { HandleRotationWithoutAcceleration(e); };

//                if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
//                {
//                    e.BACGeneral.OnButtonStartClicking.AddListenerExtend(e.RotationComp.ActionWithoutAcceleration);
//                }

//                if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
//                {
//                    e.BACGeneral.OnButtonStartTouching.AddListenerExtend(e.RotationComp.ActionWithoutAcceleration);
//                }
//            }
//        }

//        public override void RemoveListeners(object entity)
//        {
//            var e = (Filter)entity;

//            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
//            {
//                e.BACGeneral.OnButtonStartClicking.RemoveListenerExtend(e.RotationComp.ActionWithoutAcceleration);
//            }

//            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
//            {
//                e.BACGeneral.OnButtonStartTouching.RemoveListenerExtend(e.RotationComp.ActionWithoutAcceleration);
//            }
//        }
//        #endregion PUBLIC_METHODS


//        #region PRIVATE_METHODS
//        private void HandleRotationWithoutAcceleration(Filter entity)
//        {
//            var cameraRigTransform = VRSF_Components.CameraRig.transform;

//            Vector3 eyesPosition = VRSF_Components.VRCamera.transform.position;
//            Vector3 rotationAxis = new Vector3(0, entity.BACCalculations.ThumbPos.Value.x, 0);

//            cameraRigTransform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.DegreesToTurn);
//        }


//        /// <summary>
//        /// Callback for when SetupVR is setup. Setup the lsiteners.
//        /// </summary>
//        /// <param name="onSetupVR"></param>
//        private void Init(OnSetupVRReady info)
//        {
//            foreach (var e in GetEntities<Filter>())
//            {
//                if (e.BACGeneral.ActionButton != EControllersButton.TOUCHPAD)
//                {
//                    Debug.LogError("<b>[VRSF] :</b> You need to assign Left Thumbstick or Right Thumbstick to use the Rotation script. Setting CanBeUsed at false.");
//                    e.BACCalculations.CanBeUsed = false;
//                    return;
//                }

//                SetupListenersResponses(e);
//            }
//        }
//        #endregion
//    }
//}