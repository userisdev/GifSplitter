using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace GifSplitter
{
    /// <summary> Program class. </summary>
    internal static class Program
    {
        /// <summary> Defines the entry point of the application. </summary>
        /// <param name="args"> The arguments. </param>
        public static void Main(string[] args)
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

            using (Image image = Image.FromFile(imageFileInfo.FullName))
            {
                FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
                int frameCount = image.GetFrameCount(dimension);
                for (int i = 0; i < frameCount; i++)
                {
                    _ = image.SelectActiveFrame(dimension, i);

                    using (Bitmap frame = (Bitmap)image.Clone())
                    {
                        Color transparentColor = frame.GetPixel(0, 0);
                        frame.MakeTransparent(transparentColor);

                        string savePath = Path.Combine(saveDirInfo.FullName, $"{i:0000}.png");
                        if (File.Exists(savePath))
                        {
                            File.Delete(savePath);
                            Console.WriteLine($"deleted. : [{savePath}]");
                        }

                        frame.Save(savePath);

                        if (i == 0) 
                        {
                            var firstSavePath = Path.Combine(imageFileInfo.DirectoryName, $"{Path.GetFileNameWithoutExtension(imageFileInfo.Name)}.png");
                            if (File.Exists(firstSavePath))
                            {
                                File.Delete(firstSavePath);
                                Console.WriteLine($"deleted. : [{firstSavePath}]");
                            }

                            frame.Save(firstSavePath);
                        }
                    }
                }
            }

            Console.WriteLine($"saved. : [{saveDirInfo.Name}]");
        }
    }
}
