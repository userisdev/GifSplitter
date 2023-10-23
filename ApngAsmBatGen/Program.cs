using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApngAsmBatGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("not found.");
                Environment.Exit(1);
            }

            var data = File.ReadAllLines(args[0]).Select(e => e.Split(',')).Select(s => (Name: s[0], Delay: int.Parse(s[1])));

            var batPath = "apngasm_run.bat";
            var list = new List<string>();

            foreach(var item in data)
            {
                var fps = 1000 / item.Delay;
                list.Add($@"apngasm.exe {item.Name}.png ./{item.Name}/*.png {item.Delay/10} 100");
            }

            list.Add("pause");

            File.WriteAllLines(batPath, list);
        }
    }
}
