//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorkerOperation
    {
        /// <summary>
        /// The control command
        /// </summary>
        public const string ControlCommand = "CTRL";
        /// <summary>
        /// The no op
        /// </summary>
        public const int NoOp = 0;
        /// <summary>
        /// The stop
        /// </summary>
        public const int Stop = 1;
        /// <summary>
        /// The reset
        /// </summary>
        public const int Reset = 2;
        /// <summary>
        /// The restart
        /// </summary>
        public const int Restart = 3;
    }
}