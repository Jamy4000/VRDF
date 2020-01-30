namespace VRSF.Core.Controllers
{
    /// <summary>
    /// List the possible "Hand" with which the user can click or over on things.
    /// There's three of them, the left and right hand and the gaze, plus a null property
    /// </summary>
	public enum EHand : int
    {
        LEFT = 1 << 0,
        RIGHT = 1 << 1,
        NONE = 1 << 2
    }
}