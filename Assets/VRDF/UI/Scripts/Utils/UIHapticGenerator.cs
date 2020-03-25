using VRDF.Core.Controllers;
using VRDF.Core.Raycast;

namespace VRDF.UI
{
    public static class UIHapticGenerator
    {
        public static void CreateClickHapticSignal(ERayOrigin rayOrigin)
        {
            if (rayOrigin == ERayOrigin.LEFT_HAND)
                new OnHapticRequestedEvent(EHand.LEFT, 0.2f, 0.1f);
            else if (rayOrigin == ERayOrigin.RIGHT_HAND)
                new OnHapticRequestedEvent(EHand.RIGHT, 0.2f, 0.1f);
        }

        public static void CreateTouchHapticSignal(ERayOrigin rayOrigin)
        {
            if (rayOrigin == ERayOrigin.LEFT_HAND)
                new OnHapticRequestedEvent(EHand.LEFT, 0.1f, 0.075f);
            else if (rayOrigin == ERayOrigin.RIGHT_HAND)
                new OnHapticRequestedEvent(EHand.RIGHT, 0.1f, 0.075f);
        }
    }
}
