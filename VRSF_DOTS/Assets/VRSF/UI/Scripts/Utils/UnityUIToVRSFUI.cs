using UnityEngine.UI;

namespace VRSF.UI
{
    /// <summary>
    /// Static class allowing us to translate Unity UI Directions to the UIDirection of the framework
    /// </summary>
    public static class UnityUIToVRSFUI
    {
        /// <summary>
        /// Turn a Slider direction into a UIDirection
        /// </summary>
        /// <param name="direction">The UnityEngine UI Slider direction</param>
        /// <returns>The UIDirection corresponding to the slider direction</returns>
        public static EUIDirection SliderDirectionToUIDirection(Slider.Direction direction)
        {
            switch (direction)
            {
                case (Slider.Direction.TopToBottom):
                    return EUIDirection.TopToBottom;

                case (Slider.Direction.BottomToTop):
                    return EUIDirection.BottomToTop;

                case (Slider.Direction.LeftToRight):
                    return EUIDirection.LeftToRight;

                case (Slider.Direction.RightToLeft):
                    return EUIDirection.RightToLeft;

                default:
                    return EUIDirection.TopToBottom;
            }
        }

        /// <summary>
        /// Turn a Scrollbar direction into a UIDirection
        /// </summary>
        /// <param name="direction">The UnityEngine UI Scrollbar direction</param>
        /// <returns>The UIDirection corresponding to the scrollbar direction</returns>
        public static EUIDirection ScrollbarDirectionToUIDirection(Scrollbar.Direction direction)
        {
            switch (direction)
            {
                case (Scrollbar.Direction.TopToBottom):
                    return EUIDirection.TopToBottom;

                case (Scrollbar.Direction.BottomToTop):
                    return EUIDirection.BottomToTop;

                case (Scrollbar.Direction.LeftToRight):
                    return EUIDirection.LeftToRight;

                case (Scrollbar.Direction.RightToLeft):
                    return EUIDirection.RightToLeft;

                default:
                    return EUIDirection.TopToBottom;
            }
        }
    }
}