using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGallery.Messages
{
    public class ShowSplashMessage
    {
        public ShowSplashMessage(Action splashClosedAct, TimeSpan? showDelay = null)
        {
            ShowDelay = showDelay.HasValue ? showDelay.Value : TimeSpan.FromSeconds(10);
            SplashClosedAction = splashClosedAct;
        }

        public TimeSpan ShowDelay { get; private set; }
        public Action SplashClosedAction { get; private set; }
    }
}
