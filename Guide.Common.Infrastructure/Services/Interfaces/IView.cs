using Guide.Common.Infrastructure.Models.Interfaces;
using System;
using System.Windows;

namespace Guide.Common.Infrastructure.Services.Interfaces
{
    public interface IView
    {
        #region Properties

        #region Events
        event EventHandler Opening;
        event EventHandler Closing;

        event EventHandler Opened;
        event EventHandler Closed;
        #endregion

        #endregion
        bool IsOpened { get; }
        bool AutoOpen { get; }
        bool IsAnimatable { get; }
        bool IsFullPage { get; }
        double Opacity { get; set; }
        Visibility Visibility { get; set; }
        ISection Section { get; set; }

        #region Methods
        void Open();
        void Close();
        #endregion
    }
}