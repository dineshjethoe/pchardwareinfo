using System;
using System.Collections.Generic;

namespace ComputerHardwareInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> runPcs = new List<string> { "PC-1", "PC-2", "PC-3" };

            foreach (var pc in runPcs)
            {
                new ComputerInfo(pc, OutputType.File).Execute();
            }

            Console.ReadKey();
        }
    }
}
