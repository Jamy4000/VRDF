using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    [UpdateAfter(typeof(FlyBoundariesClamper))]
    public class FlyAwaySystem : ComponentSystem
    {
        private Transform _cameraRigTransform;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            OnSetupVRReady.Listeners += Init;
        }

        protected override void OnUpdate()
        {
            if (_cameraRigTransform == null)
                return;

            Entities.WithAny(typeof(IsFlying), typeof(IsDecelerating)).ForEach((ref FlyDirection direction) => 
            {
                if (direction.CurrentDirection != Vector3.zero)
                {
                    if (Vector3IsCorrect(direction.CurrentDirection))
                        _cameraRigTransform.position += direction.CurrentDirection;
                    else
                        direction.CurrentDirection = Vector3.zero;
                }
            });

            bool Vector3IsCorrect(Vector3 posToTest)
            {
                return FloatIsCorrect(posToTest.x) && FloatIsCorrect(posToTest.y) && FloatIsCorrect(posToTest.z);
            
                bool FloatIsCorrect(float toTest)
                {
                    return !float.IsInfinity(toTest) && !float.IsNaN(toTest);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= Init;
        }
        
        private void Init(OnSetupVRReady info)
        {
            _cameraRigTransform = VRSF_Components.CameraRig.transform;
        }
    }
}