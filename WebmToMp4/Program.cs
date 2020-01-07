using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace WebmToMp4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n");
            RunConversion().GetAwaiter().GetResult();
        }

        private static async Task RunConversion()
        {
            var inputDirectory = $@"{ Environment.CurrentDirectory }\input";
            var outputDirectory = $@"{ Environment.CurrentDirectory }\output";
            if (Directory.Exists(inputDirectory))
            {
                if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

                foreach (var fileName in Directory.GetFiles(inputDirectory))
                {
                    var fullFileName = Path.GetFileName(fileName);
                    var extension = Path.GetExtension(fileName);
                    if (extension.ToLower() != ".mp4")
                    {
                        string outputFileName = $@"{ outputDirectory }\{ fullFileName }";
                        var conversion = Conversion.ToMp4(fileName, Path.ChangeExtension(outputFileName, ".mp4")).SetOverwriteOutput(true);
                        conversion.OnProgress += async (sender, args) =>
                        {
                            ClearLine();
                            await Console.Out.WriteAsync($"[{args.Duration}/{args.TotalLength}][{args.Percent}%] - {fullFileName}");
                        };

                        await conversion.Start();

                        await Console.Out.WriteLineAsync($"\nFinished converion file [{fileName}]");
                    }
                }
            }
        }
        static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
