using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
namespace NoobCore
{
    public static class TypeConstants
    {
        /// <summary>
        /// Initializes the <see cref="TypeConstants"/> class.
        /// </summary>
        static TypeConstants()
        {
            ZeroTask = InTask(0);
            TrueTask = InTask(true);
            FalseTask = InTask(false);
            EmptyTask = InTask(new object());
        }

        /// <summary>
        /// Ins the task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static Task<T> InTask<T>(this T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }
        /// <summary>
        /// The zero task
        /// </summary>
        public static readonly Task<int> ZeroTask;
        /// <summary>
        /// The true task
        /// </summary>
        public static readonly Task<bool> TrueTask;
        /// <summary>
        /// The false task
        /// </summary>
        public static readonly Task<bool> FalseTask;
        /// <summary>
        /// The empty task
        /// </summary>
        public static readonly Task<object> EmptyTask;
        /// <summary>
        /// The empty object
        /// </summary>
        public static readonly object EmptyObject = new object();
        /// <summary>
        /// The non width white space
        /// </summary>
        public const char NonWidthWhiteSpace = (char)0x200B; //Use zero-width space marker to capture empty string        
        /// <summary>
        /// The non width white space chars
        /// </summary>
        public static char[] NonWidthWhiteSpaceChars = { (char)0x200B };

        /// <summary>
        /// The empty string array
        /// </summary>
        public static readonly string[] EmptyStringArray = new string[0];
        /// <summary>
        /// The empty long array
        /// </summary>
        public static readonly long[] EmptyLongArray = new long[0];
        /// <summary>
        /// The empty int array
        /// </summary>
        public static readonly int[] EmptyIntArray = new int[0];
        /// <summary>
        /// The empty character array
        /// </summary>
        public static readonly char[] EmptyCharArray = new char[0];
        /// <summary>
        /// The empty bool array
        /// </summary>
        public static readonly bool[] EmptyBoolArray = new bool[0];
        /// <summary>
        /// The empty byte array
        /// </summary>
        public static readonly byte[] EmptyByteArray = new byte[0];
        /// <summary>
        /// The empty object array
        /// </summary>
        public static readonly object[] EmptyObjectArray = new object[0];
        /// <summary>
        /// The empty type array
        /// </summary>
        public static readonly Type[] EmptyTypeArray = new Type[0];
        /// <summary>
        /// The empty field information array
        /// </summary>
        public static readonly FieldInfo[] EmptyFieldInfoArray = new FieldInfo[0];
        /// <summary>
        /// The empty property information array
        /// </summary>
        public static readonly PropertyInfo[] EmptyPropertyInfoArray = new PropertyInfo[0];

        /// <summary>
        /// The empty byte array array
        /// </summary>
        public static readonly byte[][] EmptyByteArrayArray = new byte[0][];

        /// <summary>
        /// The empty string dictionary
        /// </summary>
        public static readonly Dictionary<string, string> EmptyStringDictionary = new Dictionary<string, string>(0);
        /// <summary>
        /// The empty object dictionary
        /// </summary>
        public static readonly Dictionary<string, object> EmptyObjectDictionary = new Dictionary<string, object>();
        /// <summary>
        /// The empty object list
        /// </summary>
        public static readonly List<object> EmptyObjectList = new List<object>(0);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TypeConstants<T>
    {
        /// <summary>
        /// The empty array
        /// </summary>
        public static readonly T[] EmptyArray = new T[0];
        /// <summary>
        /// The empty list
        /// </summary>
        public static readonly List<T> EmptyList = new List<T>(0);
        /// <summary>
        /// The empty hash set
        /// </summary>
        public static readonly HashSet<T> EmptyHashSet = new HashSet<T>();
    }
}
