using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Moqs
{
    public class QuickStartTests
    {
    }

    public interface IFoo
    {
        Bar Bar { get; set; }
        string Name { get; set; }
        int Value { get; set; }
        bool DoSomething(string value);
        bool DoSomething(int number, string value);
        Task<bool> DoSomethingAsync();
        string DoSomethingStringy(string value);
        bool TryParse(string value, out string outputValue);
        bool Submit(ref Bar bar);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetCount();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(int value);
    }

    /// <summary>
    /// 
    /// </summary>
    public class Bar
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Baz Baz { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Submit() { return false; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Baz
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
    }
}
