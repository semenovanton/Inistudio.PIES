namespace Inostudio.PIES.Core
{
    /// <summary>
    /// Buffer which works as queue (FIFO).
    /// </summary>
    /// <typeparam name="TValue">Type of buffer elems</typeparam>
    public interface IBuffer<TValue>
    {
        /// <summary>
        /// Pushes a new element into buffer.
        /// </summary>
        /// <param name="value">The new element value.</param>
        void Push(TValue value);
        /// <summary>
        /// Tries to get first element from buffer.
        /// </summary>
        /// <param name="value">The extruded value. Can be NULL if buffer is empty.</param>
        /// <returns><c>TRUE</c> if element was successfully extruded, otherwise - <c>FALSE</c>.</returns>
        bool TryPull(out TValue value);
    }
}