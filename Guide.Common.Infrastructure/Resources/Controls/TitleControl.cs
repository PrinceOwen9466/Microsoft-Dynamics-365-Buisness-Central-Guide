using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class TitleControl : Control
    {
        #region Properties

        #region Events
        public event EventHandler SearchOpened;
        public event EventHandler SearchClosed;
        #endregion

        #region Internals
        AnimationTimeline ActivateSearchAnimation { get; set; }
        AnimationTimeline DeactivateSearchAnimation { get; set; }
        Storyboard ActivateSearchStory { get; set; }
        Storyboard DeactivateSearchStory { get; set; }


        ToggleButton SearchButton { get; set; }
        Panel SearchPanel { get; set; }

        TextBox SearchBox { get; set; }
        #endregion

        #region Dependency Properties


        public bool SearchActive
        {
            get { return (bool)GetValue(SearchActiveProperty); }
            set { SetValue(SearchActiveProperty, value); }
        }

        public static readonly DependencyProperty SearchActiveProperty =
            DependencyProperty.Register("SearchActive", typeof(bool), typeof(TitleControl), new UIPropertyMetadata(false, (s, e) =>
            {
                if (s is TitleControl)
                    ((TitleControl)s).AnimateSearch();
            }));


        #endregion

        #endregion

        #region Constructors
        static TitleControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleControl), new FrameworkPropertyMetadata(typeof(TitleControl)));
        public TitleControl() { }
        #endregion

        #region Methods

        #region Overrides
        public override void OnApplyTemplate()
        {
            SearchPanel = (Panel)GetTemplateChild("SearchPanel");

            ActivateSearchAnimation = ((AnimationTimeline)TryFindResource("ActivateSearchFrames")).Clone();
            DeactivateSearchAnimation = ((AnimationTimeline)TryFindResource("DeactivateSearchFrames")).Clone();

            ActivateSearchStory = new Storyboard();
            ActivateSearchStory.Children.Add(ActivateSearchAnimation);
            Storyboard.SetTarget(ActivateSearchAnimation, SearchPanel);

            DeactivateSearchStory = new Storyboard();
            DeactivateSearchStory.Children.Add(DeactivateSearchAnimation);
            Storyboard.SetTarget(DeactivateSearchAnimation, SearchPanel);

            ActivateSearchStory.Completed += (s, e) =>
            {
                SearchOpened?.Invoke(this, EventArgs.Empty);
                Keyboard.Focus(SearchBox);
            };

            DeactivateSearchStory.Completed += (s, e) => SearchClosed?.Invoke(this, EventArgs.Empty);

            SearchBox = (TextBox)GetTemplateChild("SearchBox");            

            base.OnApplyTemplate();
        }

        void AnimateSearch()
        {
            if (SearchActive)
            {
                DeactivateSearchStory.Stop();
                ActivateSearchStory.Begin();
            }
            else
            {
                ActivateSearchStory.Stop();
                DeactivateSearchStory.Begin();
            }
        }
        #endregion

        #endregion
    }
}
