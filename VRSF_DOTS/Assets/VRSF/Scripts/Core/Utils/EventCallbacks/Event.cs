using System;
using System.Collections.Generic;

namespace EventCallbacks
{
    /// <summary>
    /// Base class for the event callback system. Extend this class to create a new event to raise and listen.
    /// For a simple example, check the Github Repository at this adress :
    /// https://github.com/Jamy4000/UnityCallbackAndEventTutorial
    /// </summary>
    /// <typeparam name="T">The new Event you've created and that extend this class</typeparam>
    public class Event<T> where T : Event<T>
    {
        /// <summary>
        /// The Description of the Event, what it's supposed to do
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The delegate for the Event Listeners, create as _listeners variable
        /// </summary>
        /// <param name="info">Allow us to have the informations on the event that was fired</param>
        public delegate void EventListener(T info);

        /// <summary>
        /// The private event, that can only be modified here. This is basically the field, not the event itself.
        /// </summary>
        private static EventListener _listener;

        /// <summary>
        /// The event, working kind of like a list, that contains all the delegates to call when the event is fired.
        /// This is a wrapper for the event field, changing the add and remove method.
        /// </summary>
        public static event EventListener Listeners
        {
            add { RegisterListener(value); }
            remove { UnregisterListener(value); }
        }

        /// <summary>
        /// The list of Delegates that were added to this event (Basically what we need to call when the event is raised)
        /// </summary>
        private static List<Delegate> _listenersDelegates = new List<Delegate>();

        /// <summary>
        /// Base constructor for the Event class
        /// </summary>
        /// <param name="description">The description of this event, copied in the Description variable</param>
        public Event(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Static method to register a method that listen to this event.
        /// You need to add the type of the Event as a parameter of your method to be able to register it as a listener.
        /// </summary>
        /// <param name="listener">The method that need to be added to the listeners</param>
        /// <param name="overrideSecurityCheck">Pass true if you want to register a delegate multiple times</param>
        public static void RegisterListener(EventListener listener, bool overrideSecurityCheck = false)
        {
            if (overrideSecurityCheck || !_listenersDelegates.Contains(listener))
            {
                _listener += listener;
                _listenersDelegates.Add(listener);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError("EVENT ERROR : Cannot add a listener that was already registered.\n" +
                    "If you want to register a delegate multiple times, paqss true as a parameter of the RegisterListener method.");
#else
                System.Console.WriteLine("ERROR : Cannot add a listener that was already registered.\n" +
                    "If you want to register a delegate multiple times, paqss true as a parameter of the RegisterListener method.");
#endif
            }
        }

        /// <summary>
        /// Static method to unregister a method that was listening to this event.
        /// You need to add the type of the Event as a parameter of your method to be able to register it as a listener.
        /// </summary>
        /// <param name="listener">The method that need to be removed from the listeners</param>
        public static void UnregisterListener(EventListener listener)
        {
            _listener -= listener;

            if (_listenersDelegates.Contains(listener))
            {
                _listenersDelegates.Remove(listener);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("EVENT ERROR : The delegate with Method Name " + listener.Method.Name + " wasn't registered. Check your event registration before unregistering it.");
#else
                System.Console.WriteLine("EVENT ERROR : The delegate with Method Name " + listener.Method.Name + " wasn't registered. Check your event registration before unregistering it.");
#endif
            }
        }

        /// <summary>
        /// Whether the method you pass as parameter is already registered in this event.
        /// </summary>
        /// <param name="listener">The method/delegate that you wanna check</param>
        /// <returns>true if the delegate is already registered</returns>
        public static bool IsMethodAlreadyRegistered(EventListener listener)
        {
            return _listenersDelegates.Contains(listener);
        }

        /// <summary>
        /// Fire the event. You can put it in the constructor of your Event, or simply call it when you need it.
        /// </summary>
        /// <param name="info">The reference to the event. Can be FireEvent(this) if you call it from the Constructor of your event.</param>
        public void FireEvent(T info)
        {
            _listener?.Invoke(info);
        }
    }
}