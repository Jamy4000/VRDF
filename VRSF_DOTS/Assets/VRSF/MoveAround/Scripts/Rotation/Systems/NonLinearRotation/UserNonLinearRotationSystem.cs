using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using Unity.Entities;
using Unity.Mathematics;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
{
    /// <summary>
    /// Rotate the user based on an amount of degrees set in the editor
    /// </summary>
    public class UserNonLinearRotationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (!VRSF_Components.SetupVRIsReady)
                return;

            Entities.ForEach((ref NonLinearUserRotation nlur, ref ControllersInteractionType cit, ref BaseInputCapture bic, ref TouchpadInputCapture tic, ref InteractionThumbPosition itp) =>
            {
                if (!nlur.HasAlreadyRotated && InteractionChecker.IsInteractingTouchpad(bic, cit, itp, tic))
                {
                    VRSF_Components.RotateVRCameraAround(new float3(0.0f, tic.ThumbPosition.x, 0.0f), nlur.DegreesToRotate);
                    nlur.HasAlreadyRotated = true;
                }
                else if (nlur.HasAlreadyRotated && InteractionChecker.IsNotInteractingTouchpad(bic, cit, itp, tic))
                {
                    nlur.HasAlreadyRotated = false;
                }
            });
        }
    }
}