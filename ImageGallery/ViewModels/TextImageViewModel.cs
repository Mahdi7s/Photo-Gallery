using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using ImageGallery.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ImageGallery.Models;

namespace ImageGallery.ViewModels
{
    public class TextImageViewModel: PropertyChangedBase
    {
        private string _imageName { get; set; }
        private string _text { get; set; }
        private bool _isSelected = false;

        private readonly IEventAggregator _eventAggregator;
        private readonly TextImagesViewModel _textImagesViewModel;

        public TextImageViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();

            _textImagesViewModel = IoC.Get<TextImagesViewModel>();
            AppSettings = IoC.Get<AppSettings>();
        }

        public AppSettings AppSettings { get; private set; }

        public int Id { get; set; }
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                if(!string.IsNullOrEmpty(_imageName))
                {
                    Name = Path.GetFileName(_imageName);
                }

                NotifyOfPropertyChange(() => ImageName);
            }
        }

        public string Name { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);

                _eventAggregator.Publish(new GenericMessage { Message = "ImageSelection" });
            }
        }

        public void Edit()
        {
            _textImagesViewModel.EditingImage = this;
        }

        public void OnImageMouseDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                IsSelected = !IsSelected;
            }
            else if (e.ClickCount >= 2)
            {
                IsSelected = false;
                Edit();
            }
        }
    }
}
