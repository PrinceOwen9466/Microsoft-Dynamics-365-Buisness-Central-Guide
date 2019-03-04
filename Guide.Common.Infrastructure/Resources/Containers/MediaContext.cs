using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Guide.Common.Infrastructure.Resources.Containers
{
    public class MediaContext : BindableBase
    {
        #region Properties

        #region Dependency Properties
        public ObservableCollection<MediaPlacemark> Placemarks { get; } = new ObservableCollection<MediaPlacemark>();

        string _title = string.Empty;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        string _description = string.Empty;
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        #endregion

        #endregion

        #region Constructors
        public MediaContext()
        {
            
        }
        #endregion

        #region Methods
        #endregion
    }

    [ContentProperty("Description")]
    public class MediaPlacemark : BindableBase
    {
        #region Properties

        TimeSpan _start = TimeSpan.MinValue;
        public TimeSpan Start { get => _start; set => SetProperty(ref _start, value); }

        TimeSpan _end = TimeSpan.MinValue;
        public TimeSpan End { get => _end; set => SetProperty(ref _end, value); }

        string _title = string.Empty;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        string _description = string.Empty;
        public string Description { get => _description; set => SetProperty(ref _description, value); }


        #endregion
    }
}
