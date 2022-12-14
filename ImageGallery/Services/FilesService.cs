using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;
using TS7S.Base;

namespace ImageGallery.Services
{
    [Export]
    public class FilesService
    {
        public FilesService()
        {
            InitAssets();
        }

        private void InitAssets()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var assetsDir = Path.Combine(appDir, "Assets");
            var assetsZipPath = Path.Combine(appDir, "Assets.zip");

            if (!File.Exists(assetsZipPath))
                return;

            if (!Directory.Exists(assetsDir))
                Directory.CreateDirectory(assetsDir);

            ExtractDataTo(assetsZipPath, appDir);
            File.Delete(assetsZipPath);
        }

        public void CompactDataTo(string zipPath, string[] files, string[] directories = null)
        {
            using(ZipFile zf = new ZipFile())
            {
                zf.AlternateEncodingUsage = ZipOption.Always;
                zf.AlternateEncoding = Encoding.UTF8;

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
                zf.AlternateEncodingUsage = ZipOption.Always;
                zf.AlternateEncoding = Encoding.UTF8;
                zf.ExtractAll(extractionDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
