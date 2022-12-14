using ImageGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGallery.Messages
{
    public class GallerySelectedMessage
    {
        public GalleryViewModel Gallery { get; set; }
    }
}
