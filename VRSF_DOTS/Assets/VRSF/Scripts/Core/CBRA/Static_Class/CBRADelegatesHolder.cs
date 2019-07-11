using System;
using System.Collections.Generic;

namespace VRSF.Core.CBRA
{
    public static class CBRADelegatesHolder
    {
        public static Dictionary<CBRAInteractionType, Dictionary<ActionType, Action>> TouchEvents = new Dictionary<CBRAInteractionType, Dictionary<ActionType, Action>>();

        public static Dictionary<CBRAInteractionType, Dictionary<ActionType, Action>> ClickEvents = new Dictionary<CBRAInteractionType, Dictionary<ActionType, Action>>();
    }

    public enum ActionType
    {
        StartInteracting,
        IsInteracting,
        StopInteracting
    }
}