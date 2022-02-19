namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorkerStatus
    {
        /// <summary>
        /// The disposed
        /// </summary>
        public const int Disposed = -1;
        /// <summary>
        /// The stopped
        /// </summary>
        public const int Stopped = 0;
        /// <summary>
        /// The stopping
        /// </summary>
        public const int Stopping = 1;
        /// <summary>
        /// The starting
        /// </summary>
        public const int Starting = 2;
        /// <summary>
        /// The started
        /// </summary>
        public const int Started = 3;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="workerStatus">The worker status.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToString(int workerStatus)
        {
            switch (workerStatus)
            {
                case Disposed:
                    return "Disposed";
                case Stopped:
                    return "Stopped";
                case Stopping:
                    return "Stopping";
                case Starting:
                    return "Starting";
                case Started:
                    return "Started";
            }
            return "Unknown";
        }
    }
}