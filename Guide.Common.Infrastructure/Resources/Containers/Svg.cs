using Guide.Common.Infrastructure.Extensions;
using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Containers
{
    public class Svg : DependencyObject, IUriContext
    {
        #region Properties

        #region IUriContext Implementation
        public Uri BaseUri { get; set; }
        #endregion

        #region Dependency Properties



        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Svg), new PropertyMetadata(false));



        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(Svg), new PropertyMetadata(null, (s, e) =>
            {
                if (s is Svg && e.NewValue is Uri)
                    ((Svg)s).RenderImage((Uri)e.NewValue);
            }));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Svg), new UIPropertyMetadata(null, (s, e) =>
            {
                if (s is Svg) ((Svg)s).RenderImage(((Svg)s).Source);
            }));

        public SvgDocument Document
        {
            get { return (SvgDocument)GetValue(DocumentProperty); }
            private set { SetValue(DocumentProperty, value); }
        }
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(SvgDocument), typeof(Svg), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Svg), new PropertyMetadata(null));


        #endregion

        #endregion

        #region Methods
        public void Activate() => RenderImage(Source);

        void RenderImage(Uri source)
        {
            try
            {
                Stream svgStream = null;

                Uri folder = new Uri(BaseUri, ".");
                source = new Uri(folder, source.OriginalString);

                if (source.ToString().IndexOf("siteoforigin", StringComparison.OrdinalIgnoreCase) >= 0)
                    svgStream = Application.GetRemoteStream(source).Stream;
                else svgStream = Application.GetResourceStream(source).Stream;

                using (svgStream)
                {
                    Document = SvgDocument.Open<SvgDocument>(svgStream);

                    if (Fill is SolidColorBrush)
                    {
                        var color = ((SolidColorBrush)Fill).Color.ToDrawingColor();
                        Document.Fill = new SvgColourServer(color);
                    }
                    ImageSource = BitmapHelper.ToBitmapSource(Document.Draw());
                }
                IsActive = true;
            }
            catch (Exception ex)  { Core.Log.Error($"An error occured while rendering an svg image\n{ex}"); }
        }
        #endregion
    }
}
