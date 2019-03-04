using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.Interfaces
{
    public interface IShell
    {
        WindowState WindowState { get; set; }

        void DragMove();
        void Close();
    }
}
