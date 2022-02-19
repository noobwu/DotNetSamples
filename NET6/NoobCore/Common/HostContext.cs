using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostContext
    {
        /// <summary>
        /// Gets the application host.
        /// </summary>
        /// <value>
        /// The application host.
        /// </value>
        public static ServiceStackHost AppHost => ServiceStackHost.Instance;
    }
}
