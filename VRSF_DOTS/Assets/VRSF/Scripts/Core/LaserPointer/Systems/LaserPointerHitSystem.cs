using Unity.Entities;
using Unity.Jobs;
using VRSF.Core.Events;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// System to handle the visibility of the Pointers based on whether it's hitting something
    /// </summary>
    public class LaserPointerHitSystem : JobComponentSystem
    {
        private CheckObjectHitJob _objectHoveredJobHandle;

        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _objectHoveredJobHandle = new CheckObjectHitJob
            {
                RayOrigin = ERayOrigin.NONE,
                ObjectHoveredIsNull = true
            };

            // Just check if there's at least one entity
            if (GetEntityQuery(typeof(LaserPointerVisibility)).CalculateLength() > 0)
                ObjectWasHoveredEvent.Listeners += OnSomethingWasHit;

            this.Enabled = false;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (inputDeps.IsCompleted)
                this.Enabled = false;

            return _objectHoveredJobHandle.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            if (ObjectWasHoveredEvent.IsMethodAlreadyRegistered(OnSomethingWasHit))
                ObjectWasHoveredEvent.Listeners -= OnSomethingWasHit;

            base.OnDestroy();
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        private void OnSomethingWasHit(ObjectWasHoveredEvent info)
        {
            _objectHoveredJobHandle = new CheckObjectHitJob
            {
                RayOrigin = info.RaycastOrigin,
                ObjectHoveredIsNull = info.ObjectHovered == null
            };
            this.Enabled = true;
        }
        
        /// <summary>
        /// Set the state of the pointer based on the hits 
        /// </summary>
        struct CheckObjectHitJob : IJobForEach<LaserPointerState, VRRaycastOrigin>
        {
            public ERayOrigin RayOrigin;
            public bool ObjectHoveredIsNull;

            public void Execute(ref LaserPointerState stateComp, ref VRRaycastOrigin raycastOrigin)
            {
                if (raycastOrigin.RayOrigin == RayOrigin && !ObjectHoveredIsNull)
                    stateComp.State = EPointerState.ON;
            }
        }
        #endregion PRIVATE_METHODS
    }
}