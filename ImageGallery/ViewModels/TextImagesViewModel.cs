using Caliburn.Micro;
using ImageGallery.Messages;
using ImageGallery.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using TS7S.Base;

namespace ImageGallery.ViewModels
{
    [Export]
    public class TextImagesViewModel : PropertyChangedBase, IHandle<GallerySelectedMessage>
    {
        private GalleryViewModel _galleryViewModel = null;
        private OpenFileDialog _openFileDlg = null;
        private TextImageViewModel _editingImage = null;

        [ImportingConstructor]
        public TextImagesViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            _openFileDlg = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                RestoreDirectory = true,
                Title = "Select your image",
                Multiselect = true,
                Filter = "(*.jpg)|*.jpg"
            };
            IsEditingItem = Visibility.Collapsed;
        }

        [Import]
        public AppSettings AppSettings { get; set; }

        public GalleryViewModel Gallery
        {
            get { return _galleryViewModel; }
            private set
            {
                _galleryViewModel = value;
                NotifyOfPropertyChange(() => Gallery);
                NotifyOfPropertyChange(() => Images);
                //NotifyOfPropertyChange(() => CanAdd);
                //NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public Visibility IsEditingItem { get; private set; }
        public TextImageViewModel EditingImage
        {
            get { return _editingImage; }
            set
            {
                _editingImage = value;
                IsEditingItem = _editingImage != null ? Visibility.Visible : Visibility.Collapsed;

                NotifyOfPropertyChange(() => EditingImage);
                NotifyOfPropertyChange(() => IsEditingItem);
            }
        }

        public ObservableCollection<TextImageViewModel> Images
        {
            get { return (Gallery != null) ? Gallery.Images : null; }
        }

        //public bool CanAdd
        //{
        //    get { return Gallery != null; }
        //}

        //public void Add()
        //{
        //    if (_openFileDlg.ShowDialog() == true)
        //    {
        //        var files = _openFileDlg.FileNames;
        //        if (!files.IsNullOrEmpty())
        //        {
        //            foreach (var fName in files)
        //            {
        //                Gallery.Images.Add(new TextImageViewModel { Id = _imagesIdentifier++, ImageName = fName });
        //            }
        //        }
        //    }
        //}

        public void CloseEdit()
        {
            EditingImage = null;
        }

        public void Handle(GallerySelectedMessage message)
        {
            Gallery = message.Gallery;
        }
    }
}
