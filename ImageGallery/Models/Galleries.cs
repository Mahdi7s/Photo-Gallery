using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGallery.Models
{
    [Serializable]
    public class Galleries
    {
        public List<Gallery> ImageGalleries { get; set; }
    }
}
