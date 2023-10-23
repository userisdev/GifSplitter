using Aspose.Imaging.FileFormats.Apng;
using Aspose.Imaging.FileFormats.Png;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using Aspose.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApngGen
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
            foreach (var item in data)
            {
                Run(new FileInfo(args[0]).Directory, item.Name, item.Delay);
            }
        }

        private static void Run(DirectoryInfo dir, string name, int delay)
        {
            var targetDir = new DirectoryInfo(Path.Combine(dir.FullName, name));
            if (!targetDir.Exists)
            {
                Console.WriteLine($"not found. : {name}");
                return;
            }

            var savePath = Path.Combine(dir.FullName, $"{name}.png");


            var files = targetDir.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(f => f.Name).ToArray();
            foreach (var file in files)
            {
            }

            int w = 0;
            int h = 0;
            using (var img = System.Drawing.Image.FromFile(files.First().FullName))
            {
                w = img.Width;
                h = img.Height;
            }

            if (w <= 0 || h <= 0)
            {
                Console.WriteLine("read error.");
                return;
            }

            ApngOptions createOptions = new ApngOptions
            {
                Source = new FileCreateSource(savePath, false),
                DefaultFrameTime = (uint)delay,
                ColorType = PngColorType.TruecolorWithAlpha,
            };

            using (ApngImage apngImage = (ApngImage)Image.Create(createOptions, w, h))
            {
                apngImage.RemoveAllFrames();

                foreach (var file in files)
                {
                    var img = (RasterImage)Image.Load(file.FullName);
                    apngImage.AddFrame(img, (uint)delay);
                }


                apngImage.Save();
            }
        }
    }
}
