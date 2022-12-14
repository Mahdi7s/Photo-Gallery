using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGallery.Models
{
    [Serializable]
    public class TextImage
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Text { get; set; }
    }
}

