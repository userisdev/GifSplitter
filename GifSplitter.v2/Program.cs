using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;

namespace GifSplitter.v2
{
    internal class Program
    {
        /// <summary> Runs the specified path. </summary>
        /// <param name="path"> The path. </param>
        private static void Run(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"not found. : [{path}]");
                return;
            }

            FileInfo imageFileInfo = new FileInfo(path);
            string saveDirPath = Path.Combine(imageFileInfo.DirectoryName, Path.GetFileNameWithoutExtension(imageFileInfo.Name));
            DirectoryInfo saveDirInfo = new DirectoryInfo(saveDirPath);
            saveDirInfo.Create();

            using (var image = Image.FromFile(imageFileInfo.FullName))
            {

                var gd = new GifDecoder(image);
                Console.WriteLine($"loop count : {gd.LoopCount}");
                for(int i = 0;i < gd.FrameCount; i++)
                {
                    var frame = gd.GetFrame(image, i);

                    var savePath = Path.Combine(saveDirInfo.FullName, $"{i:0000}.png");
                    frame.Image.Save(savePath);
                }

            }


                Console.WriteLine($"saved. : [{saveDirInfo.Name}]");
        }

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("few args.");
                Environment.Exit(1);
            }

            string path = args.FirstOrDefault();
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine("not found.");
                Environment.Exit(1);
            }

            if (File.Exists(path))
            {
                FileInfo imageFileInfo = new FileInfo(path);
                if (!imageFileInfo.Exists || !imageFileInfo.Extension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("args error.");
                    Environment.Exit(1);
                }

                Run(imageFileInfo.FullName);
            }

            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly).Where(e => e.Extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (!files.Any())
                {
                    Console.WriteLine("not found.");
                    Environment.Exit(1);
                }

                foreach (FileInfo fileInfo in files)
                {
                    Run(fileInfo.FullName);
                }
            }

            Environment.Exit(0);
        }
    }
}
