namespace Inostudio.PIES.Core
{
    /// <summary>
    /// Buffer, that writes its values into the file in ascending order.
    /// </summary>
    /// <typeparam name="TValue">Type of the elements</typeparam>
    public interface IOutputFileBuffer<TValue>
    {
        #region public properties
        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Gets the buffer size. 
        /// <para>This size should depend on how disordered elements are pushed into the buffer.</para>
        /// </summary>
        int BufferSize { get; }
        #endregion

        #region public methods
        /// <summary>
        /// Pushes a new element into buffer.
        /// </summary>
        /// <param name="value">The new element value.</param>
        void Push(TValue value);
        /// <summary>
        /// Flushes whole buffer into file in proper order.
        /// </summary>
        void Flush();
        #endregion
    }
}
