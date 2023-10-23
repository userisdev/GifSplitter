using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GifGen.v2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("not found.");
                Environment.Exit(1);
            }

            var data = File.ReadAllLines(args[0]).Select(e => e.Split(',')).Select(s => (Name: s[0], Delay: int.Parse(s[1])));
            foreach(var item in data)
            {
                Run(new FileInfo(args[0]).Directory, item.Name, item.Delay);
            }

        }

        private static void Run(DirectoryInfo dir ,string name,int delay )
        {
            var targetDir = new DirectoryInfo(Path.Combine(dir.FullName, name));
            if (!targetDir.Exists ) {
                Console.WriteLine($"not found. : {name}");
                return;
            }

            var savePath = Path.Combine(dir.FullName, $"{name}.gif");
            var ge = new GifEncoder();


            var files = targetDir.GetFiles("*", SearchOption.TopDirectoryOnly).ToArray();
            foreach(var file in files )
            {
                var gifFrame = new GifFrame();
                gifFrame.Image = Image.FromFile(file.FullName);
                gifFrame.Delay = TimeSpan.FromMilliseconds(delay);
                gifFrame.X = 0;
                gifFrame.Y = 0;
                ge.AddFrame(gifFrame);
            }


            var img = ge.Save();
            img.Save(savePath);
        }
    }
}
