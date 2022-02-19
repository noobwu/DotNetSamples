namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorkerStatus
    {
        /// <summary>
        /// 
        /// </summary>
        public const int Disposed = -1;
        /// <summary>
        /// 
        /// </summary>
        public const int Stopped = 0;
        /// <summary>
        /// 
        /// </summary>
        public const int Stopping = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int Starting = 2;
        /// <summary>
        /// 
        /// </summary>
        public const int Started = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerStatus"></param>
        /// <returns></returns>
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