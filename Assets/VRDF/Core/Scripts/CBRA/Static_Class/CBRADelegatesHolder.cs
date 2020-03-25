using System;
using System.Collections.Generic;
using Unity.Entities;

namespace VRDF.Core.CBRA
{
    /// <summary>
    /// Keep a reference to the events to call, linked to each CBRA Entity.
    /// </summary>
    public static class CBRADelegatesHolder
    {
        /// <summary>
        /// Dictionary containing all StartTouching Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> StartTouchingEvents = new Dictionary<Entity, Action>();

        /// <summary>
        /// Dictionary containing all IsTouching Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> IsTouchingEvents = new Dictionary<Entity, Action>();

        /// <summary>
        /// Dictionary containing all StopTouching Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> StopTouchingEvents = new Dictionary<Entity, Action>();


        /// <summary>
        /// Dictionary containing all StartClicking Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> StartClickingEvents = new Dictionary<Entity, Action>();

        /// <summary>
        /// Dictionary containing all IsClicking Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> IsClickingEvents = new Dictionary<Entity, Action>();

        /// <summary>
        /// Dictionary containing all StopClicking Listeners for every CBRA entities.
        /// </summary>
        public static Dictionary<Entity, Action> StopClickingEvents = new Dictionary<Entity, Action>();
    }
}