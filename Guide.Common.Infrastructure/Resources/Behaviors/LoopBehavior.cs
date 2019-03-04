using Guide.Common.Infrastructure.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Guide.Common.Infrastructure.Resources.Behaviors
{
    public class LoopBehavior : Behavior<MediaElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MediaEnded += (s, e) =>
            {
                if (!(AssociatedObject is ResourceMediaPlayer))
                    AssociatedObject.Position = new TimeSpan(0, 0, 1);
            };
        }
    }
}
