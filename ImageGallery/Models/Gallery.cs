using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGallery.Models
{
    [Serializable]
    public class Gallery
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TextImage> Images { get; set; }
        public List<Gallery> ChildGalleries { get; set; }
    }
}
