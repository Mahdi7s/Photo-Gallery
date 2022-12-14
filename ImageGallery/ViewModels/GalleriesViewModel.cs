using Caliburn.Micro;
using ImageGallery.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using ImageGallery.Models;
using System.Windows.Forms;
using TS7S.Base;

namespace ImageGallery.ViewModels
{
    [Export]
    public class GalleriesViewModel: PropertyChangedBase, IHandle<GenericMessage>
    {
        private string _searchText = string.Empty;
        private IEventAggregator _eventAggregator;
        private OpenFileDialog _openFileDlg;
        private GalleryViewModel _selectedGallery = null;
        private int _imagesIdentifier = 0;

        private int _galleryIdentifier = 0;
        private ObservableCollection<GalleryViewModel> _galleries;

        [ImportingConstructor]
        public GalleriesViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Galleries = new ObservableCollection<GalleryViewModel>();
            _galleries.CollectionChanged +=
                (s, e) =>
                {
                    _eventAggregator.Publish(new GenericMessage { Message = "GalleriesChanged" });
                };

            _openFileDlg = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                RestoreDirectory = true,
                Title = "Select your image",
                Multiselect = true,
                Filter = "(*.jpg)|*.jpg"
            };
        }

        [Import]
        public AppSettings AppSettings { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
                NotifyOfPropertyChange(()=>Galleries);
            }
        }

        public void ClearGalleries()
        {
            _galleries.Clear();
            NotifyOfPropertyChange(() => Galleries);
        }

        public ObservableCollection<GalleryViewModel> Galleries
        {
            get
            {
                var retval = new ObservableCollection<GalleryViewModel>(_galleries.Where(x => SearchTest(x, _searchText)));
                //if (retval.Any()) retval.First().IsExpanded = true;

                return retval;
            }
            private set
            {
                _galleries = value;
                NotifyOfPropertyChange(() => Galleries);
            }
        }

        private bool SearchTest(GalleryViewModel galleryViewModel, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var isMatched1 = galleryViewModel.Name.ToLower().Contains(text.ToLower());
                var isMatched2 = false;

                galleryViewModel.MatchedSearch = isMatched1;

                foreach (var gallery in galleryViewModel.ChildGalleries)
                {
                    galleryViewModel.IsExpanded = gallery.MatchedSearch = SearchTest(gallery, text);
                    isMatched2 = isMatched2 || gallery.MatchedSearch;
                }

                return isMatched1 || isMatched2;
            }
            else
            {
                galleryViewModel.MatchedSearch = false;
                galleryViewModel.IsExpanded = false;

                foreach (var gallery in galleryViewModel.ChildGalleries)
                {
                    gallery.MatchedSearch = false;
                    gallery.IsExpanded = false;
                }

                return true;
            }
        }

        public void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            _selectedGallery = e.NewValue as GalleryViewModel;
            NotifyOfPropertyChange(() => CanAddImage);
            NotifyOfPropertyChange(() => CanSelectAllImages);
            NotifyOfPropertyChange(() => CanDeleteImages);

            _eventAggregator.Publish(new GallerySelectedMessage { Gallery = _selectedGallery });
        }

        public void AddGallery()
        {
            SearchText = string.Empty;

            _galleries.Add(new GalleryViewModel { Parent = this, Id = _galleryIdentifier++, Name = "گالری جدید", GalleryMode = GalleryMode.Edit });
            NotifyOfPropertyChange(() => Galleries);
        }

        public void AddGallery(GalleryViewModel galleryViewModel)
        {
            _galleries.Add(galleryViewModel);
            NotifyOfPropertyChange(() => Galleries);
        }

        public void RemoveGallery(GalleryViewModel gallery)
        {
            _galleries.Remove(gallery);
                NotifyOfPropertyChange(() => Galleries);
        }

        public bool CanAddImage
        {
            get { return _selectedGallery != null; }
        }

        public void AddImage()
        {
            if (_openFileDlg.ShowDialog() == DialogResult.OK)
            {
                var files = _openFileDlg.FileNames;
                if (!files.IsNullOrEmpty())
                {
                    foreach (var fName in files)
                    {
                        _selectedGallery.Images.Add(new TextImageViewModel { Id = _imagesIdentifier++, ImageName = fName });
                    }
                    NotifyOfPropertyChange(() => CanSelectAllImages);
                }
            }
        }

        public void Handle(GenericMessage message)
        {
            if (message.Message == "ImageSelection")
            {
                NotifyOfPropertyChange(() => CanDeleteImages);
                NotifyOfPropertyChange(() => CanSelectAllImages);
            }
        }

        public bool CanDeleteImages
        {
            get { return _selectedGallery != null && _selectedGallery.Images.Any(x => x.IsSelected); }
        }

        public void DeleteImages()
        {
            var imagesToDelete = _selectedGallery.Images.Where(x => x.IsSelected).ToArray();
            foreach (var img in imagesToDelete)
            {
                _selectedGallery.RemoveImage(img);
            }
        }

        public bool CanSelectAllImages
        {
            get { return _selectedGallery != null && !_selectedGallery.Images.IsNullOrEmpty(); }
        }

        public void SelectAllImages()
        {
            foreach (var img in _selectedGallery.Images)
            {
                img.IsSelected = true;
            }
        }
    }
}
