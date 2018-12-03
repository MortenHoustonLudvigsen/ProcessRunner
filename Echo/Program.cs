using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo
{
    class Program
    {
        static Program()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void Main(string[] args)
        {
            foreach (var arg in args.Select((a, i) => new { Value = a, Index = i }))
            {
                Console.WriteLine("args[{0}]: \"{1}\"", arg.Index, arg.Value);
            }

            foreach (var line in ReadInputLines())
            {
                Console.WriteLine("Line: \"{0}\"", line);
            }
        }

        private static IEnumerable<string> ReadInputLines()
        {
            var line = Console.ReadLine();
            while (line != null)
            {
                yield return line;
                line = Console.ReadLine();
            }
        }
    }
}
