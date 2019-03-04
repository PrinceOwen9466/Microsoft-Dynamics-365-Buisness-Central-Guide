using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Guide.Demos.Presentation.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Properties

        #region Statics
        const string VideoPath = @"C:\Users\Prince\Videos\2018-11-01 13-33-18.mp4";
        const string PagePath = @"C:\Users\Prince\Documents\Web Development\Classroom\Head First Web Development\Book 1\CH13 - Seagway'n USA\index.html";
        #endregion

        #region Bindables
        public string MediaSource { get; private set; } = VideoPath;

        Models.Page _page = new Models.Page("<html><body>Hello World</body></html>");
        public Models.Page Page { get => _page; private set => SetProperty(ref _page, value); }
        //public ObservableCollection<>
        #endregion

        #region Commands
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        #endregion

        #region Internals

        #endregion

        #endregion

        #region Constructors
        public ShellViewModel()
        {
            PlayCommand = new DelegateCommand<object>(OnPlay);
            StopCommand = new DelegateCommand<object>(OnStop);
        }
        #endregion

        #region Methods

        void OnPlay(object obj)
        {
            if (obj is WebBrowser)
            {
                WebBrowser browser = ((WebBrowser)obj);
                browser.Source = new Uri(PagePath);
            }


            if (!(obj is MediaElement)) return;
            MediaElement element = (MediaElement)obj;
            element.Play(); 
        }

        void OnStop(object obj)
        {
            if (!(obj is MediaElement)) return;
            ((MediaElement)obj).Stop();
        }

        #endregion
    }
}
