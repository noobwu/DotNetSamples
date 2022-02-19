using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
namespace NoobCore.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReturn { }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReturn<T> : IReturn { }
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Interfaces.IReturn" />
    public interface IReturnVoid : IReturn { }
}
