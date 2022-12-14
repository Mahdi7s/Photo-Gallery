using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace ImageLibrary.Installer
{
    [RunInstaller(true)]
    public class AssetsInstaller : System.Configuration.Install.Installer
    {
        private readonly FilesService _filesService;

        public AssetsInstaller()
        {
            _filesService = new FilesService();
        }

        public override void Install(IDictionary stateSaver)
        {
            //var appDir = (string)stateSaver["APPDIR"];
            //var assetsDir = Path.Combine(appDir, "Assets");
            //var assetsZipPath = Path.Combine(appDir, "Assets.zip");

            //if (!File.Exists(assetsZipPath))
            //    throw new InstallException("The file Assets.zip does not exists in directory: " + appDir);

            //if (!Directory.Exists(assetsDir))
            //    Directory.CreateDirectory(assetsDir);

            //_filesService.ExtractDataTo(assetsZipPath, assetsDir);
            //File.Delete(assetsZipPath);

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }
}
