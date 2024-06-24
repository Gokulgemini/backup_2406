namespace RDM.Data.ImageVault
{
    /// <summary>
    /// Represents the result of a database read operation.
    /// </summary>
    /// <typeparam name="T">The type of the value to wrap on success.</typeparam>
    public class ReadResult<T>
    {
        private ReadResult(ReadStatus status, T value)
        {
            Status = status;
            Value = value;
        }

        /// <summary>
        /// The result value when successful.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The status of the read.
        /// </summary>
        public ReadStatus Status { get; }

        /// <summary>
        /// Creates a new success result.
        /// </summary>
        /// <param name="value">The value to wrap on success.</param>
        /// <returns>A success result.</returns>
        public static ReadResult<T> Ok(T value)
        {
            return new ReadResult<T>(ReadStatus.Success, value);
        }

        /// <summary>
        /// Creates a new not found failure result.
        /// </summary>
        /// <returns>A failure result.</returns>
        public static ReadResult<T> NotFound()
        {
            return new ReadResult<T>(ReadStatus.NotFound, default(T));
        }
    }
}
