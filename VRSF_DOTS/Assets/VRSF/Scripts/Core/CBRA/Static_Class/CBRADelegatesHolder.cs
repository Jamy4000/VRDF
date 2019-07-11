using System;
using System.Collections.Generic;
using Unity.Entities;

namespace VRSF.Core.CBRA
{
    public static class CBRADelegatesHolder
    {
        public static Dictionary<Entity, Dictionary<ActionType, Action>> TouchEvents = new Dictionary<Entity, Dictionary<ActionType, Action>>();

        public static Dictionary<Entity, Dictionary<ActionType, Action>> ClickEvents = new Dictionary<Entity, Dictionary<ActionType, Action>>();
    }

    public enum ActionType
    {
        StartInteracting,
        IsInteracting,
        StopInteracting
    }
}