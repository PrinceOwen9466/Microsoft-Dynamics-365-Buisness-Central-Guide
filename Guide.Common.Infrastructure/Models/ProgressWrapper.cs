using Guide.Common.Infrastructure.Models.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Models
{
    public class ProgressWrapper : BindableBase
    {
        #region Properties
        double _progress = .0;
        public double Progress { get => _progress; set => SetProperty(ref _progress, value); }

        ISection _section = null;
        public ISection Section { get => _section; set => SetProperty(ref _section, value); }
        #endregion

        #region Methods

        public void RaiseChanges()
        {
            RaisePropertyChanged(nameof(Section));
            RaisePropertyChanged(nameof(Progress));
        }

        #endregion
    }
}
