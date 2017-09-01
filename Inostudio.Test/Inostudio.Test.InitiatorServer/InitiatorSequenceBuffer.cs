using Inostudio.Test.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inostudio.Test.InitiatorServer
{
    /// <summary>
    /// Dummy sender buffer, which generates numbers.
    /// </summary>
    class InitiatorSequenceBuffer 
        : IBuffer<long>
    {
        #region public methods
        /// <summary>
        /// Pushes new element into buffer.
        /// </summary>
        /// <param name="value">The new element value.</param>
        public void Push(long value)
        {
            throw new NotImplementedException("This method is not allowed for this buffer");
        }

        /// <summary>
        /// Tries to get first element from buffer.
        /// </summary>
        /// <param name="value">The extruded value. Can be NULL if buffer is empty.</param>
        /// <returns><c>TRUE</c> if element was successfully extruded, otherwise - <c>FALSE</c>.</returns>
        public bool TryPull(out long value)
        {
            //use lock for safe concorrent access
            lock (_syncLock)
            {
                value = ++_lastNumber;
                return true;
            }
        }
        #endregion

        #region private fields
        private long _lastNumber = 0;
        private object _syncLock = new object();
        #endregion
    }
}
