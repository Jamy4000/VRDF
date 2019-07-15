using Unity.Mathematics;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    public static class ThumbPositionChecker
    {
        /// <summary>
        /// Check if the position of the finger on the controller correspond to the one specified in the Editor
        /// </summary>
        /// <param name="posToCheck">The thumb position required to call the delegate method</param>
        /// <param name="buttonEvent">The Action to call if the thumbPos is on the good position</param>
        /// <param name="threshold">The minimum value on the controller thumbstick (between -1 and 1) to call the delegate method</param>
        /// <returns>true if the thumb is at the good position</returns>
        public static bool CheckThumbPosition(EThumbPosition posToCheck, float threshold, float2 thumbPos)
        {
            // If the position to check is ANY, and at least one of the four position is more than the threshold in the both axis
            if (posToCheck == EThumbPosition.ANY)
                return true;

            // If the position to check contains at least LEFT, we check if if the pos value is < to the threshold in the x axis
            if ((posToCheck & EThumbPosition.LEFT) == EThumbPosition.LEFT && thumbPos.x <= -threshold)
                return true;

            // If the position to check contains at least RIGHT, we check if if the pos value is > to the threshold in the x axis
            if ((posToCheck & EThumbPosition.RIGHT) == EThumbPosition.RIGHT && thumbPos.x >= threshold)
                return true;

            // If the position to check contains at least UP, we check if if the pos value is > to the threshold in the y axis
            if ((posToCheck & EThumbPosition.UP) == EThumbPosition.UP && thumbPos.y >= threshold)
                return true;

            // If the position to check contains at least DOWN, we check if if the pos value is < to the threshold in the y axis
            if ((posToCheck & EThumbPosition.DOWN) == EThumbPosition.DOWN && thumbPos.y <= -threshold)
                return true;

            return false;
        }
    }
}