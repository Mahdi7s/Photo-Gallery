using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Caliburn.Micro;
using ImageGallery.Messages;

namespace ImageGallery
{
    [Export()]
    public class SplashViewModel : Screen, IHandle<ShowSplashMessage>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SplashViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public void Handle(ShowSplashMessage message)
        {
            var timer = new DispatcherTimer
                            {
                                Interval = message.ShowDelay,
                            };
            timer.Tick += (s, e) =>
                              {
                                  timer.Stop();

                                  TryClose();
                                  message.SplashClosedAction();
                              };

            _windowManager.ShowWindow(this);
            timer.Start();
        }
    }
}
