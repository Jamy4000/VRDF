using Unity.Entities;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Extend this class to be able to setup the listeners of the BAC Systems.
    /// The SetupListenersResponses need to be called in the start method and the RemoveListeners in a destroy or disable method
    /// </summary>
    public abstract class BACListenersSetupSystem : ComponentSystem
    {
        // Here's an example of a Filter struct you can use for your system
        // The Filter of your setup system need to at least have a BACGeneralComponent
        //
        //struct Filter
        //{
        //    public T1 Type1;
        //    public T2 Type2;
        //    public Tn TypeN;
        //    public BACGeneralComponent BAC_Comp;
        //}

        /// <summary>
        /// Method called to setup the listeners Responses by using delegate or lambda expression
        /// </summary>
        /// <param name="entity"></param>
        public abstract void SetupListenersResponses(object entity);

        // Here's an example of the implementation of SetupListenersResponses.
        // The Filter of yoursetup system need to at least have a BACGeneralComponent
        //
        //public override void SetupListenersResponses(object entity)
        //{
        //    var e = (Filter)entity;
        //    if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
        //    {
        //        e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartClickingCallback(e); });
        //        e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { OnIsClickingCallback(e);});
        //        e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { OnStopClickingCallback(e); });
        //    }

        //    if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
        //    {
        //        e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartTouchingCallback(e); });
        //        e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { OnIsTouchingCallback(e);});
        //        e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { OnStopTouchingCallback(e); });
        //    }
        //}

        public abstract void RemoveListeners(object entity);

        // Here's an example of the implementation of RemoveListeners.
        // The Filter of yoursetup system need to at least have a BACGeneralComponent
        //
        //public override void RemoveListeners(object entity)
        //{
        //    var e = (Filter)entity;
        //    // If the interaction enum flag contains click
        //    if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
        //    {
        //        e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
        //        e.BAC_Comp.OnButtonIsClicking.RemoveAllListeners();
        //        e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
        //    }

        //    // If the interaction enum flag contains touch
        //    if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
        //    {
        //        e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
        //        e.BAC_Comp.OnButtonIsTouching.RemoveAllListeners();
        //        e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
        //    }
        //}

        // Example implementation of the Start Method to setup the listeners
        //
        //protected override void OnStartRunning()
        //{
        //    base.OnStartRunning();
        //    foreach (var e in GetEntities<Filter>())
        //    {
        //        SetupListenersResponses(e);
        //    }
        //}

        // Example implementation of the Stop/Destroy/Disable Method to remove the listeners
        //
        //protected override void OnDestroyManager()
        //{
        //    base.OnDestroyManager();
        //    foreach (var e in GetEntities<Filter>())
        //    {
        //        RemoveListeners(e);
        //    }
        //}


        protected override void OnUpdate() { }
    }
}