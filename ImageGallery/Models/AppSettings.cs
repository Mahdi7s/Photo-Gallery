using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;

namespace ImageGallery.Models
{
    [Export]
    public class AppSettings
    {
        public AppSettings()
        {
            var appSettings = ConfigurationSettings.AppSettings;

            IsPublisher = appSettings["IsPublisher"].Equals("true", StringComparison.OrdinalIgnoreCase)
                              ? Visibility.Visible
                              : Visibility.Collapsed;
        }

        public Visibility IsPublisher { get; private set; }
    }
}
