using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Guide.Desktop.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Properties

        #region Services
        ILoggerFacade Logger { get; }
        IShell Shell { get; }
        IPresenter Presenter { get; }
        #endregion

        #region Commands
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        #endregion

        #endregion

        #region Constructors
        public ShellViewModel(ILoggerFacade logger, IShell shell, IPresenter presenter)
        {
            Logger = logger;
            Shell = shell;
            Presenter = presenter;


            PreviousCommand = new DelegateCommand(OnPrevious);
            NextCommand = new DelegateCommand(OnNext);
        }
        #endregion

        #region Methods


        #region Command Handlers
        void OnPrevious()
        {
            //SlideBarActive = false;
            Presenter.SwipeDirection = Direction.Left;
            Presenter.Previous();
        }

        void OnNext()
        {
            Presenter.SwipeDirection = Direction.Right;
            Presenter.Next();
        }
        #endregion

        #endregion
    }
}
