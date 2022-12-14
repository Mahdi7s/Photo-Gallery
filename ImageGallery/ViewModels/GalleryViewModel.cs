using Caliburn.Micro;
using ImageGallery.Messages;
using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ImageGallery.ViewModels
{
    public enum GalleryMode { Normal, Edit, Delete }

    public class GalleryViewModel: PropertyChangedBase, IDataErrorInfo
    {
        private IEventAggregator _eventAgg = null;

        private string _name = string.Empty;
        private GalleryMode _galleryMode = GalleryMode.Normal;
        private bool _isExpanded = false;
        private bool _isSelected = false;
        private Visibility _isInNormalMode = Visibility.Collapsed;
        private Visibility _isInEditMode = Visibility.Visible;
        private bool _matchedSearch = false;

        private int _imagesIdentifier = 0;
        private int _galleriesIdentifier = 0;

        private readonly GalleriesViewModel _galleriesViewModel=null;
        
        public GalleryViewModel()
        {
            ChildGalleries = new ObservableCollection<GalleryViewModel>();
            Images = new ObservableCollection<TextImageViewModel>();

            _eventAgg = IoC.Get<IEventAggregator>();
            _galleriesViewModel = IoC.Get<GalleriesViewModel>();
            AppSettings = IoC.Get<AppSettings>();
        }

        public AppSettings AppSettings { get; private set; }

        public int Id { get; set; }

        //[Required]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool MatchedSearch
        {
            get { return _matchedSearch; }
            set
            {
                _matchedSearch = value;
                NotifyOfPropertyChange(() => MatchedSearch);
            }
        }

        public dynamic Parent { get; set; }
        public ObservableCollection<GalleryViewModel> ChildGalleries { get; private set; }
        public ObservableCollection<TextImageViewModel> Images { get; private set; }

        public GalleryMode GalleryMode
        {
            get { return _galleryMode; }
            set
            {
                _galleryMode = value;
                switch (_galleryMode)
                {
                    case GalleryMode.Normal:
                        IsInNormalMode = Visibility.Visible;
                        IsInEditMode = Visibility.Collapsed;
                        break;
                    case GalleryMode.Edit:
                        IsInNormalMode = Visibility.Collapsed;
                        IsInEditMode = Visibility.Visible;
                        break;
                }
            }
        }

        public Visibility IsInNormalMode
        {
            get { return _isInNormalMode; }
            private set
            {
                _isInNormalMode = value;
                NotifyOfPropertyChange(() => IsInNormalMode);
            }
        }

        public Visibility IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                NotifyOfPropertyChange(() => IsInEditMode);
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value; NotifyOfPropertyChange(() => IsExpanded);
            }
        }

        public bool IsSelected { 
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(_name))
            {
                GalleryMode = GalleryMode.Normal;
                _eventAgg.Publish(new GenericMessage { Message = "GalleriesChanged" });
            }
        }

        public void Clear()
        {
            Name = string.Empty;
        }

        public void Edit()
        {
            GalleryMode = GalleryMode.Edit;
        }

        public void Add()
        {           
            ChildGalleries.Add(new GalleryViewModel { Parent=this,Id= _galleriesIdentifier++, Name = "گالری جدید", GalleryMode = GalleryMode.Edit });
            IsExpanded = true;
        }

        public void AddGallery(GalleryViewModel galleryViewModel)
        {
            ChildGalleries.Add(galleryViewModel);
        }

        public void Remove(GalleryViewModel gallery)
        {
            if (gallery.Parent != null)
            {
                gallery.Parent.RemoveGallery(gallery);
            }
        }

        public void RemoveGallery(GalleryViewModel gallery)
        {
            ChildGalleries.Remove(gallery);
        }

        public void ImageNameKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save();
            }
        }

        //--------------------------------------------------------------------------------

        public void AddImage(TextImageViewModel textImage)
        {
            textImage.Id = _imagesIdentifier++;
            Images.Add(textImage);
        }

        public void RemoveImage(TextImageViewModel image)
        {
            Images.Remove(image);
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get {
                if (columnName == "Name")
                {
                    if (string.IsNullOrWhiteSpace(_name))
                        return "Fill this!";
                }

                return null;
            }
        }
    }
}
