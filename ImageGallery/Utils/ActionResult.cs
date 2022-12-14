using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Action = System.Action;

namespace ImageGallery.Utils
{
    public class ActionResult : IResult
    {
        private readonly Action _actionToExecute = null;
        public ActionResult(Action actionToExecute)
        {
            if(_actionToExecute == null)
                throw new ArgumentNullException("actionToExecute");

            _actionToExecute = actionToExecute;
        }

        public void Execute(ActionExecutionContext context)
        {
            if(context.CanExecute == null || context.CanExecute())
            {
                _actionToExecute();
                Completed(this, new ResultCompletionEventArgs());
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}
