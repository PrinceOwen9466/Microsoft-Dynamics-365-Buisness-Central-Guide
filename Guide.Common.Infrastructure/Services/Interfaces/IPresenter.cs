using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
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
        bool FullPageActive { get; }
        IPresentable Content { get; }
        Direction SwipeDirection { get; set; }
        #endregion

        #region Methods
        void Initialize(IPresentable content, IView home);
        void Previous();
        void Next();
        void RefreshFullView();
        #endregion
    }
}
