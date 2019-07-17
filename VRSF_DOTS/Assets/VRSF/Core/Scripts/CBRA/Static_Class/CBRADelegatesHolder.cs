using System;
using System.Collections.Generic;
using Unity.Entities;

namespace VRSF.Core.CBRA
{
    public static class CBRADelegatesHolder
    {
        public static Dictionary<Entity, Action> StartTouchingEvents = new Dictionary<Entity, Action>();
        public static Dictionary<Entity, Action> IsTouchingEvents = new Dictionary<Entity, Action>();
        public static Dictionary<Entity, Action> StopTouchingEvents = new Dictionary<Entity, Action>();

        public static Dictionary<Entity, Action> StartClickingEvents = new Dictionary<Entity, Action>();
        public static Dictionary<Entity, Action> IsClickingEvents = new Dictionary<Entity, Action>();
        public static Dictionary<Entity, Action> StopClickingEvents = new Dictionary<Entity, Action>();
    }
}