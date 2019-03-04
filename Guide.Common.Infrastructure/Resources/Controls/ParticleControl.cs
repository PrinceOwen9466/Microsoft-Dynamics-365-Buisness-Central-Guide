using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Guide.Common.Infrastructure.Extensions;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class ParticleControl : ContentControl
    {
        #region Properties

        #region Internals

        #region Elements
        Canvas Canvas { get; }
        ParticleEmitter Emitter { get; }
        DrawingVisualElement Renderer { get; }

        #endregion

        #endregion

        #endregion

        #region Constructors
        public ParticleControl()
        {
            Canvas = new Canvas() { ClipToBounds = true };
            Content = Canvas;

            Emitter = new ParticleEmitter() { Container = Canvas };
            Renderer = new DrawingVisualElement();

            CompositionTarget.Rendering += (s, e) =>
            {
                if (!IsVisible) return;
                Emitter.Update();

                Canvas.Children.Clear();

                using (var dc = Renderer.visual.RenderOpen())
                {
                    //dc.DrawRectangle(
                    //    Brushes.Black,
                    //    null,
                    //    new Rect(
                    //        new Size(
                    //            canvas.ActualWidth,
                    //            canvas.ActualHeight)));

                    Random rand = new Random();
                    Emitter.particles.ForEach(p =>
                    {

                        var c = p.Color;

                        c.A /= 2;
                        var diameter = rand.Next(0, 10);

                        dc.DrawEllipse(
                            new SolidColorBrush(c),
                            null, p.Position, 5, 5);

                        /*
                        dc.DrawEllipse(
                            new SolidColorBrush(p.Color),
                            null, p.Position, 1, 1);
                        */
                    });
                }

                Canvas.Children.Add(Renderer);
            };
        }
        #endregion

    }

    #region Extra Classes
    class DrawingVisualElement : FrameworkElement
    {
        public DrawingVisual visual;

        public DrawingVisualElement() { visual = new DrawingVisual(); }

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index) { return visual; }
    }

    public class Particle
    {
        public Point Position;
        public Point Velocity;
        public Color Color;
        public double Elapsed;

        public void Update(double elapsedSeconds, Rect bounds)
        {
            Elapsed += elapsedSeconds;

            //if (Position.X > )
            /*
            if (Elapsed > Lifespan)
            {
                Color.A = 0;
                return;
            }*/
            //Color.A = (byte)(255 - ((255 * Elapsed)) / Lifespan);
            bounds = new Rect(bounds.X, bounds.Y, bounds.Width + 100, bounds.Height + 100);

            Position.X += Velocity.X * elapsedSeconds;
            Position.Y += Velocity.Y * elapsedSeconds;
            if (!bounds.Contains(Position))
                Color.A = 0;
        }
    }

    public class ParticleEmitter
    {
        public Point Center { get; set; }
        public List<Particle> particles = new List<Particle>();
        Random rand = new Random();
        public WriteableBitmap TargetBitmap;
        public WriteableBitmap ParticleBitmap;
        public FrameworkElement Container { get; set; }

        void CreateParticle()
        {
            if (particles.Count >= 200) return;

            var speed = rand.Next(50) + 100;
            var angle = GetRandomNumber(rand, 170, 190);//2 * Math.PI * rand.NextDouble();

            double width = Container.ActualWidth;
            double height = Container.ActualHeight;

            // Get a random position at the bottom
            Point position = new Point(x: GetRandomNumber(rand, 0, width), y: height + 50);

            particles.Add(
                new Particle()
                {
                    Position = position,
                    Velocity = new Point(Math.Sin(angle) * speed, -1 * speed),
                    Color = Color.FromRgb(255, 255, 255),
                });
        }

        public void Update()
        {
            var updateInterval = .005;

            CreateParticle();

            particles.RemoveAll(p =>
            {
                p.Update(updateInterval, GetBounds(Container));
                return p.Color.A == 0;
            });
        }

        public double GetRandomNumber(Random rand, double minimum, double maximum) => rand.NextDouble() * (maximum - minimum) + minimum;


        Rect GetBounds(FrameworkElement element)
        {
            //element.FindParent<Window>()
            var parent = element.FindParent<FrameworkElement>();
            if (parent == null)
                return new Rect();
            return element.TransformToVisual(parent).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
            //return element.TransformToAncestor(Application.Current.MainWindow).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
        }
    }
    #endregion
}
