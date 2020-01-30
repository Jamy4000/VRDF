using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using VRSF.Core.Inputs;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.Teleport
    {
        /// <summary>
        /// 
        /// </summary>
        public class TeleportStatusSystem : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                return new TeleportStatusJob().Schedule(this, inputDeps);
            }

            [Unity.Burst.BurstCompile]
            private struct TeleportStatusJob : IJobForEach<GeneralTeleportParameters, ControllersInteractionType, BaseInputCapture>
            {
                public void Execute(ref GeneralTeleportParameters gtp, [ReadOnly] ref ControllersInteractionType cit, [ReadOnly] ref BaseInputCapture bic)
                {
                    switch (gtp.CurrentTeleportState)
                    {
                        case ETeleportState.None:
                            if (InteractionChecker.IsInteractingSimple(bic, cit))
                                gtp.CurrentTeleportState = ETeleportState.Selecting;
                            break;
                        case ETeleportState.Selecting:
                            if (InteractionChecker.IsNotInteracting(bic, cit))
                            {
                                gtp.CurrentTeleportState = ETeleportState.Teleporting;
                                gtp.HasTeleported = false;
                            }
                            break;
                        case ETeleportState.Teleporting:
                            if (gtp.HasTeleported)
                                gtp.CurrentTeleportState = ETeleportState.None;
                            break;
                    }
                }
            }
        }
    }