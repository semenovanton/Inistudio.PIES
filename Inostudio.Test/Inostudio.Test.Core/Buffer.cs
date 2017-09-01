using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Base implementation for inner server buffer. See <seealso cref="IBuffer"/>
    /// <para>FIFO</para>
    /// </summary>
    /// <typeparam name="TValue">Type of buffer elems</typeparam>
    public class Buffer<TValue> : IBuffer<TValue>
    {
        #region public methods
        /// <summary>
        /// Pushes new element into buffer.
        /// </summary>
        /// <param name="value">The new element value.</param>
        public virtual void Push(TValue value)
        {
            _queue.Enqueue(value);
        }

        /// <summary>
        /// Tries to get first element from buffer.
        /// </summary>
        /// <param name="value">The extruded value. Can be NULL if buffer is empty.</param>
        /// <returns><c>TRUE</c> if element was successfully extruded, otherwise - <c>FALSE</c>.</returns>
        public bool TryPull(out TValue value)
        {
            return _queue.TryDequeue(out value);
        }
        #endregion

        #region protected fields
        /// <summary>
        /// stores all elements
        /// <para>use <seealso cref="ConcurrentQueue"/> for multithead safety access.</para>
        /// </summary>
        protected ConcurrentQueue<TValue> _queue = new ConcurrentQueue<TValue>();
        #endregion
    }
}
