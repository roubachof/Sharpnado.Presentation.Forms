namespace Sharpnado.Presentation.Forms.Paging
{
    /// <summary>
    /// Interface IInfiniteListLoader.
    /// </summary>
    public interface IInfiniteListLoader
    {
        /// <summary>
        /// This method must be called by the UI element in charge of displaying data.
        /// Per example, on android, a scroll listener can reference IInfiniteListLoader and call it from OnScroll.
        /// The implementation execution time of this method must be transparent as it should return immediately and doesn't block the caller.
        /// </summary>
        /// <param name="lastVisibleIndex">Index of the last visible item.</param>
        void OnScroll(int lastVisibleIndex);
    }
}