using System;
using System.IO;
using System.IO.Compression;
using Repositories.Interfaces.Downloader;

namespace Core.Implementation.Downloader
{
    public class Processer : IProcesser
    {
        public void ProcessGz(string filepath, string saveas)
        {
            byte[] file = File.ReadAllBytes(filepath);

            Console.WriteLine("****** Start Decompress Feed ******");
            Console.WriteLine(Environment.NewLine);

            using (var stream = new GZipStream(new MemoryStream(file), CompressionMode.Decompress))
            {
                const int size = 32758;
                //byte[] buffer = new byte[size];
                byte[] buffer = new byte[32768 - 10];

                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        Console.Write(".");
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("****** End Decompress Feed ******");

                    File.WriteAllBytes(saveas, memory.ToArray()); // Requires System.IO
                }
            }
        }
    }
}
