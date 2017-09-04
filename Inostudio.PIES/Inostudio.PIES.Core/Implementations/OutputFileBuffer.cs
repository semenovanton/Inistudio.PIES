using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inostudio.PIES.Core.Implementations
{
    /// <summary>
    /// Base inmplementation of the <seealso cref="IFlushingBuffer"/>, that writes its values into the file in ascending order.
    /// </summary>
    /// <typeparam name="TValue">Type of the elements</typeparam>
    public class OutputFileBuffer<TValue>
        : IOutputFileBuffer<TValue>
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FlushingBuffer"/>.
        /// </summary>
        /// <param name="fileName">The name of the ouput file.</param>
        /// <param name="bufferSize">Buffer size.</param>
        public OutputFileBuffer(string fileName, int bufferSize)
        {
            FileName = fileName;
            BufferSize = bufferSize;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        public string FileName { get; protected set; }
        /// <summary>
        /// Gets the buffer size. 
        /// <para>This size should depend on how disordered elements are pushed into the buffer.</para>
        /// </summary>
        public int BufferSize { get; protected set; }
        #endregion

        #region public methods
        /// <summary>
        /// Pushes a new element into buffer.
        /// </summary>
        /// <param name="value">The new element value.</param>
        public void Push(TValue value)
        {
            lock (_lock)
            {
                _bag.Add(value);

                if (_bag.Count >= BufferSize)
                {
                    //this action is danger even if we have concurrent collection.

                    //we should be sure, that only one thread will take this value
                    if (TryGetNext(out TValue nextValue))
                    {
                        //we should be sure, that only one thread will write into file.
                        WriteToFile(new TValue[] { nextValue });
                    }
                }
            }
        }
        /// <summary>
        /// Flushes whole buffer into file in proper order.
        /// </summary>
        public void Flush()
        {
            //this action is danger even if we have concurrent collection.
            lock (_lock)
            {
                //we should be sure, that only one thread will take these values
                var lastValues = new List<TValue>();
                while (TryGetNext(out TValue nextValue))
                {
                    lastValues.Add(nextValue);
                }
                //we should be sure, that only one thread will write into file.
                WriteToFile(lastValues);
            }
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Tries to get next value in proper order (the minimal one).
        /// </summary>
        /// <param name="value">The minimal value in the collection. Can be <c>EMPTY OBJECT</c>if collection is empty.</param>
        /// <returns></returns>
        protected bool TryGetNext(out TValue value)
        {
            //TODO: more relevant check and possibly removing missed values to remove possible artifacts 
            //for instance: K+10 value before K value, only K+10 will be populated into file
            if (_bag.Count > 0 )
            {
                value = _bag.Min();

                _bag.Remove(value);

                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Writes selected values into the output file.
        /// </summary>
        /// <param name="values">Values to be written into the file.</param>
        protected void WriteToFile(IEnumerable<TValue> values)
        {
            using (var outFile = System.IO.File.Open(FileName, System.IO.FileMode.Append, System.IO.FileAccess.Write))
            {
                var valuesString = string.Join("", values.Select(x => $"{x}\r\n"));
                var bytes = Encoding.UTF8.GetBytes(valuesString);
                outFile.Write(bytes, 0, bytes.Length);
            }
        }
        #endregion

        #region protected fields
        /// <summary>
        /// use for concurrent working with minimal values and output files.
        /// </summary>
        protected object _lock = new object();
        /// <summary>
        /// concurrent collection for concurrent safety access.
        /// <para><c>Key</c> - stored value.</para>
        /// <para><c>Value</c> - the flag, indicates was this element was already flushed into file or not.</para>
        /// </summary>
        protected List<TValue> _bag = new  List<TValue>();
        #endregion

    }
}
