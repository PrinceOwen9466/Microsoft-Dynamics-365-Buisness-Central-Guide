using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class ResourceMediaPlayer : MediaElement
    {
        #region Properties

        #region Dependency Properties
        public string ResourcePath
        {
            get { return (string)GetValue(ResourcePathProperty); }
            set { SetValue(ResourcePathProperty, value); }
        }

        public static readonly DependencyProperty ResourcePathProperty =
            DependencyProperty.Register("ResourcePath", typeof(string), typeof(ResourceMediaPlayer), new UIPropertyMetadata(string.Empty));



        public Assembly ResourceAssembly
        {
            get { return (Assembly)GetValue(ResourceAssemblyProperty); }
            set { SetValue(ResourceAssemblyProperty, value); }
        }
        public static readonly DependencyProperty ResourceAssemblyProperty =
            DependencyProperty.Register("ResourceAssembly", typeof(Assembly), typeof(ResourceMediaPlayer), new UIPropertyMetadata(null));




        public bool Loop
        {
            get { return (bool)GetValue(LoopProperty); }
            set { SetValue(LoopProperty, value); }
        }
        public static readonly DependencyProperty LoopProperty =
            DependencyProperty.Register("Loop", typeof(bool), typeof(ResourceMediaPlayer), new UIPropertyMetadata(true));



        #endregion


        #endregion

        #region Constructors

        public ResourceMediaPlayer()
        {
            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;

                Source = new Uri(ResourceHelper.CreateLocalInstance(ResourcePath, ResourceAssembly), UriKind.Absolute);
            };

            Unloaded -= (s, e) =>
            {
                ResourceHelper.DissolveLocalInstance(Source.OriginalString);
            };

            MediaEnded += (s, e) =>
            {
                if (Loop) Position = new TimeSpan(0, 0, 1);
            };

            PreviewMouseLeftButtonDown += (s, e) =>
            {
                /*
                if (IsFull)
                {
                    RippleControl parent = this.FindParent<RippleControl>();
                    //parent.
                    //parent.
                }*/
            };
            
        }

        #endregion

        #region Methods
        #endregion
    }
}
