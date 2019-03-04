using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Models.Interfaces
{
    public interface IPresentable
    {
        #region Properties
        IView View { get; set; }
        int Index { get; }
        int Total { get; }
        bool CanNext { get; }
        bool CanPrevious { get; }
        ObservableCollection<ISection> Sections { get; }
        #endregion

        #region Methods
        Task Previous();
        Task Next();
        void OpenSection(ISection section);
        #endregion
    }
}
