using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class CurveTeleporterUpdateSystem : ComponentSystem
    {
        ///// <summary>
        ///// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        ///// </summary>
        //public void TeleportUser(ITeleportFilter teleportFilter)
        //{
        //    var e = (Filter)teleportFilter;

        //    // If the user has decided to teleport (ie lets go of touchpad) then remove all visual indicators
        //    // related to selecting things and actually teleport
        //    if (e.PointerCalculations.PointOnNavMesh)
        //        new OnTeleportUser(e.TeleportGeneral, e.SceneObjects);
        //    else
        //        TeleportUserSystem.SetTeleportState(ETeleportState.None, e.TeleportGeneral);
        //}

        protected override void OnUpdate()
        {

        }

        ///// <summary>
        ///// This is the callback for when the user start clicking on the button. We check if the timer is ok if we use one.
        ///// </summary>
        ///// <param name="e"></param>
        //private void OnStartInteractingCallback(Filter e)
        //{
        //    if (TeleportGeneralComponent.CanTeleport)
        //        // We reset the current State to Selecting
        //        TeleportUserSystem.SetTeleportState(ETeleportState.Selecting, e.TeleportGeneral);
        //}

        //private void OnStopInteractingCallback(Filter e)
        //{
        //    if (TeleportGeneralComponent.CanTeleport)
        //    {
        //        TeleportGeneralComponent.PointToGoTo = e.PointerCalculations.TempPointToGoTo;
        //        TeleportUser(e);
        //    }
        //}
    }
}