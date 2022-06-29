using System.Diagnostics;

namespace BeautyProgramming.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
        static void MakeUsage(float level) {
            PerformanceCounter counter = new PerformanceCounter("Processor", "");
        }
    }
}