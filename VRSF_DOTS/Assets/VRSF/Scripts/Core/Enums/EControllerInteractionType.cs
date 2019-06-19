namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all possible interaction type for Oculus and Vive controllers
    /// In our case, only concerns the Touch and Click Interaction
    /// </summary>
    public enum EControllerInteractionType
    {
        NONE = 0,
        TOUCH = 1 << 0,
        CLICK = 1 << 1,
        ALL = ~0
    }
}