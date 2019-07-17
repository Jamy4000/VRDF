namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all possible position of the thumb for the ButtonActionChoser
    /// TODO Add it to te BAC, only used there
    /// </summary>
	public enum EThumbPosition
    {
        NONE = 0,
        UP = 1 << 0,
        DOWN = 1 << 1,
        LEFT = 1 << 2,
        RIGHT = 1 << 3,
        ANY = ~0
    }
}