using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Resources.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Services.Interfaces
{
    public interface IPresenter
    {
        #region Properties
        bool FullPageActive { get; set; }
        bool VideoActive { get; set; }
        IPlayer Player { get; set; }
        IPresentable Content { get; }
        Direction SwipeDirection { get; set; }
        bool IsHome { get; set; }
        event EventHandler Initialized;
        #endregion

        #region Methods
        void Initialize(IPresentable content, IView home);
        void Previous();
        void Next();
        void RefreshFullView();
        void NavigateHome();
        void NavigateToSection(Section section);
        void NavigateToPage(Page page);
        #endregion

    }
}
