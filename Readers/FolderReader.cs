using System.Security.Cryptography;

namespace GameStarBackend.Readers
{
    public class FolderReader
    {
        static public string GetFileName()
        {
            var tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GS-Keys"));

            var keyFiles = Directory.EnumerateFiles($@"{tempFolder.FullName}");

            if (keyFiles.Any())
            {
                foreach (var file in keyFiles)
                {
                    return file;
                }
            }
            else
            {
                Console.WriteLine("Folder is empty.");
            }
            return null;
        }

        static public void CleanFolder()
        {
            var tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GS-Keys"));
            var keyFiles = Directory.EnumerateFiles($@"{tempFolder.FullName}");

            if (keyFiles.Any())
            {
                foreach (var file in keyFiles)
                {
                    Console.WriteLine("===== Deleted =====");
                    Console.WriteLine(new FileInfo(file).Name);
                    Console.WriteLine("===================");
                    File.Delete(file);
                }
            }
            else
            {
                Console.WriteLine("Nothing to clean.");
                return;
            }
        }
    }
}
