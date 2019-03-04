using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class NoClipGrid : Grid
    {
        #region Methods

        protected override Geometry GetLayoutClip(Size layoutSlotSize) => null;

        #endregion
    }
}
