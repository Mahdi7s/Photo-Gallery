using System.IO;
using System.Windows.Forms;
using Caliburn.Micro;
using ImageGallery.Messages;
using ImageGallery.Models;
using ImageGallery.Services;
using ImageGallery.Utils;
using ImageGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TS7S.Base.IO;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;


using Screen = Caliburn.Micro.Screen;

namespace ImageGallery
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel: Screen, IShell, IHandle<GenericMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _winManager;
        private readonly FilesService _filesService;
        private WindowState _windowState = WindowState.Normal;

        private readonly SaveDialogViewModel _saveDialogVm;

        private bool? _isDirty = false;
        private bool _isSaved = true;

        private string _savePath = string.Empty;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator, SaveDialogViewModel saveDlg, IWindowManager winManager, GalleriesViewModel galleriesViewModel, TextImagesViewModel textImagesViewModel, SplashViewModel splashViewModel)
        {
            DisplayName = "Irsa Image Gallery";

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _saveDialogVm = saveDlg;

            Galleries = galleriesViewModel;
            TextImages = textImagesViewModel;

            _filesService = IoC.Get<FilesService>();

            AppSettings = IoC.Get<AppSettings>();
            if(AppSettings.IsPublisher != Visibility.Visible)
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var galleryGif = Path.Combine(appPath, "Assets\\Gallery.gi");
                if(File.Exists(galleryGif))
                {
                    LoadGalleryFrom(galleryGif);
                    _eventAggregator.Publish(new GallerySelectedMessage{ Gallery = Galleries.Galleries.FirstOrDefault()});
                }
            }

            System.Windows.Application.Current.Exit += (s, e) =>
            {
                e.ApplicationExitCode = Environment.ExitCode;
            };
        }

        public string ShellTitle { get { return AppSettings.IsPublisher == Visibility.Visible ? "ویرایشگر گالری عکس" : "گالری عکس"; } }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public AppSettings AppSettings { get; private set; }

        public bool? IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                NotifyOfPropertyChange(()=>IsDirty);
                NotifyOfPropertyChange(()=>CanSave);
                NotifyOfPropertyChange(()=>CanNew);
                NotifyOfPropertyChange(()=>CanLoad);
            }
        }

        public TextImagesViewModel TextImages { get; private set; }
        public GalleriesViewModel Galleries { get; private set; }

        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }

        public void MinWinState()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void MaxWinState()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        public void OnMouseLeftButtonDown(ShellView view, MouseButtonEventArgs e)
        {
            view.DragMove();
        }

        public bool CanNew { get { return _isDirty != true; } }
    
        public void New()
        {
            MessageBoxResult res = MessageBoxResult.No;

            if (CanSave)
            {
                res = ShowSaveGalleryMessage();
                if (res == MessageBoxResult.Yes)
                {
                    if (!Save())
                    {
                        New();
                    }
                }
            }

            if (res != MessageBoxResult.Cancel)
            {
                _savePath = string.Empty;
                Galleries.ClearGalleries();
            }
        }

        private MessageBoxResult ShowSaveGalleryMessage()
        {
            return MessageBox.Show(GetView() as Window, "آیا مایل به ذخیره گالری هستید؟", "ذخیره گالری", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public bool CanSave { get { return (_isDirty != true && Galleries.Galleries.Any() && !_isSaved); } }

        public bool Save()
        {
            var galleryDes = _savePath;
            if (string.IsNullOrEmpty(galleryDes))
            {
                var saveDlg = new SaveFileDialog
                {
                    RestoreDirectory = true,
                    CheckFileExists= false,
                    CheckPathExists=false,
                    Title="select save destination",
                    Filter="(*.gi)|*.gi"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    galleryDes = saveDlg.FileName;
                    _savePath = galleryDes;
                }
            }

            if (IsDirty == true || string.IsNullOrEmpty(galleryDes)) return false;
            else
            {
                SaveTo(galleryDes);
                
                _isSaved = true;

                return true;
            }
        }

        private void SaveTo(string galleryDes)
        {
            IsDirty = true;
            //yield return new ActionResult(() => {});

            var rootPath = Path.GetDirectoryName(galleryDes);
            var galleriesPath = Path.Combine(rootPath, "Data");

            var galleriesToSave = new Galleries
                                      {
                                          ImageGalleries =
                                              Galleries.Galleries.Select(x => ToGalleryModel(x, galleriesPath)).ToList()
                                      };

            ObjectSerializer<Galleries>.SerializeBinary(galleryDes, galleriesToSave);

            IsDirty = null;
            //yield return new ActionResult(() => IsDirty = null);
        }

        public bool CanLoad { get { return _isDirty != true; } }

        public void Load()
        {
            var folderBrowser = new OpenFileDialog
                                    {
                                        RestoreDirectory = true,
                                        CheckFileExists = true,
                                        CheckPathExists = true,
                                        Title = "Select file destination",
                                        Filter = "(*.gi)|*.gi"
                                    };
            if(folderBrowser.ShowDialog() == true)
            {
                if(string.IsNullOrEmpty(folderBrowser.FileName)) return;
                var gifPath = folderBrowser.FileName;

                LoadGalleryFrom(gifPath);

                _savePath = gifPath;
            }
        }

        public bool CanMakeSetup { get { return _isDirty != true; } }
        public void MakeSetup()
        {
            var fbDlg = new FolderBrowserDialog
                            {
                                ShowNewFolderButton = true
                            };
            if(fbDlg.ShowDialog() == DialogResult.OK)
            {
                var dir = fbDlg.SelectedPath;
                CopySetupToDirectory(dir);

                SaveTo(Path.Combine(dir, "Assets\\Gallery.gi"));

                var assetsDir = Path.Combine(dir, "Assets");
                var assetsZipPath = Path.Combine(dir, "Assets.zip");

                _filesService.CompactDataTo(assetsZipPath, null, new[] {assetsDir});
            }
        }

        private void CopySetupToDirectory(string dir)
        {
            var setupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setup");
            DirecoryHelper.CopyDirectory(setupDir, dir);
        }

        private void LoadGalleryFrom(string gifPath)
        {
            var loadedGalleries = ObjectSerializer<Galleries>.DeserializeBinary(gifPath);
            Galleries.ClearGalleries();

            var gifDir = Path.GetDirectoryName(gifPath);

            foreach (var imageGallery in loadedGalleries.ImageGalleries)
            {
                LoadGallery(Path.Combine(gifDir, "Data"), imageGallery, Galleries);
            }
        }

        private void LoadGallery(string rootPath, Gallery gallery, dynamic parent)
        {
            var galleryVM = new GalleryViewModel()
                                {Id = gallery.Id, GalleryMode = GalleryMode.Normal, Name = gallery.Name, Parent = parent};
            var galleryPath = Path.Combine(rootPath, gallery.Name);

            foreach(var img in gallery.Images)
            {
                galleryVM.AddImage(new TextImageViewModel {Id = img.Id, ImageName = Path.Combine(galleryPath,img.ImageName), Text = img.Text});
            }

            galleryVM.Parent.AddGallery(galleryVM);

            foreach (var childGallery in gallery.ChildGalleries)
            {
                LoadGallery(galleryPath, childGallery, galleryVM);
            }
        }

        private Gallery ToGalleryModel(GalleryViewModel gallery, string rootPath)
        {
            var gFolderPath = Path.Combine(rootPath, gallery.Name);
            if (!Directory.Exists(gFolderPath))
                Directory.CreateDirectory(gFolderPath);

            var retval = new Gallery {Id = gallery.Id, Name = gallery.Name, ChildGalleries = new List<Gallery>(), Images = new List<TextImage>()};

            foreach (var image in gallery.Images)
            {
                if (image.ImageName != null)
                {
                    var imageName = Path.GetFileName(image.ImageName);
                    var imgNewPath = Path.Combine(gFolderPath, imageName);

                    if(!File.Exists(imgNewPath))
                        File.Copy(image.ImageName, imgNewPath, true);

                    retval.Images.Add(new TextImage {Id = image.Id, ImageName = imageName, Text = image.Text});
                }
            }

            foreach (var childGallery in gallery.ChildGalleries)
            {
                retval.ChildGalleries.Add(ToGalleryModel(childGallery, gFolderPath));
            }

            return retval;
        }

        public void Exit()
        {
            if (AppSettings.IsPublisher == Visibility.Visible)
            {
                MessageBoxResult res = MessageBoxResult.No;
                if (CanSave)
                {
                    res = ShowSaveGalleryMessage();

                    if (res == MessageBoxResult.Yes)
                    {
                        if (!Save())
                        {
                            Exit();
                        }
                    }
                }

                if (res != MessageBoxResult.Cancel)
                {
                    Environment.Exit(Environment.ExitCode);
                }
            }
            else
            {
                Environment.Exit(Environment.ExitCode);
            }
        }

        public void Handle(GenericMessage message)
        {
            if(message.Message=="GalleriesChanged")
            {
                _isSaved = false;
                NotifyOfPropertyChange(()=>CanSave);
            }
        }

        public void OnClosing(object e)
        {

        }
    }
}
