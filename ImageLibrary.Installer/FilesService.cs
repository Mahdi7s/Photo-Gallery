using System.IO;
using Ionic.Zip;
using TS7S.Base;
using CompressionLevel = Ionic.Zlib.CompressionLevel;

namespace ImageLibrary.Installer
{
    public class FilesService
    {
        public void CompactDataTo(string zipPath, string[] files, string[] directories = null)
        {
            using(ZipFile zf = new ZipFile())
            {
                zf.CompressionLevel = CompressionLevel.None;

                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);
                    zf.AddDirectory(directory, dirInfo.Name);
                }

                if (!files.IsNullOrEmpty())
                {
                    zf.AddFiles(files);
                }

                zf.Save(zipPath);
            }
        }

        public void ExtractDataTo(string zipPath, string extractionDirectory)
        {
            using (var zf = ZipFile.Read(zipPath))
            {
                zf.ExtractAll(extractionDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
