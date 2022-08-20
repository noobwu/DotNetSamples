// ***********************************************************************
// Assembly         : NoobCore
// Author           : Administrator
// Created          : 06-29-2022
//
// Last Modified By : Administrator
// Last Modified On : 06-29-2022
// ***********************************************************************
// <copyright file="GlobalHelper.cs" company="NoobCore">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Helper
{
    /// <summary>
    /// Class GlobalHelper. This class cannot be inherited.
    /// </summary>
    public sealed class GlobalHelper
    {
        /// <summary>
        /// Gets the name of the machine.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetMachineName() {
            try
            {
                return Dns.GetHostName();
            }
            catch 
            {
                return string.Empty;
            }
        }
    }
}
