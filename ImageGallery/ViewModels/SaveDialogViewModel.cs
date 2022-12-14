using Caliburn.Micro;
using ImageGallery.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace ImageGallery.ViewModels
{
    [Export(typeof(SaveDialogViewModel))]
    public class SaveDialogViewModel : Screen
    {
        private IEventAggregator _egg = null;

        [ImportingConstructor]
        public SaveDialogViewModel(IEventAggregator egg)
        {
            _egg = egg;
        }

        public event EventHandler DialogClosed = delegate { };

        public void Yes()
        {
            DialogClosed("Save-Yes", EventArgs.Empty);
            
            TryClose();
        }

        public void No()
        {
            DialogClosed("Save-No", EventArgs.Empty);
            TryClose();
        }

        public void Cancel()
        {
            DialogClosed("Save-Cancel", EventArgs.Empty);
            TryClose();
        }
    }
}
