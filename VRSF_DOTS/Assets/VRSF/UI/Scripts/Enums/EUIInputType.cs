namespace VRSF.UI
{
    /// <summary>
    /// List of UIInputType possible (Click or Over)
    /// </summary>
    public enum EUIInputType
    {
        NONE = 0,
        OVER = 1 << 0,
        CLICK = 1 << 1,
        ALL = ~0
    }
}