using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    public abstract partial class ServiceStackHost:IDisposable
    {
        /// <summary>
        /// Singleton access to AppHost
        /// </summary>
        public static ServiceStackHost Instance { get; protected set; }

        /// <summary>
        /// Executes OnDisposeCallbacks and Disposes IDisposable's dependencies in the IOC &amp; reset singleton states
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Instance = null;
            }
            //clear unmanaged resources here
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceStackHost"/> class.
        /// </summary>
        ~ServiceStackHost()
        {
            Dispose(false);
        }
    }
}
