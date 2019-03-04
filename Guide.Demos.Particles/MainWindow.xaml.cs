using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Guide.Demos.Particles
{
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
        public double Lifespan;
        public double Elapsed;

        public void Update(double elapsedSeconds)
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
            Position.X += Velocity.X * elapsedSeconds;
            Position.Y += Velocity.Y * elapsedSeconds;
        }
    }

    public class ParticleEmitter
    {
        public Point Center { get; set; }
        public List<Particle> particles = new List<Particle>();
        Random rand = new Random();
        public WriteableBitmap TargetBitmap;
        public WriteableBitmap ParticleBitmap;

        void CreateParticle()
        {
            var speed = rand.Next(20) + 140;
            var angle = 2 * Math.PI * rand.NextDouble();

            particles.Add(
                new Particle()
                {
                    Position = new Point(Center.X, Center.Y),
                    Velocity = new Point(
                        Math.Sin(angle) * speed,
                        Math.Cos(angle) * speed),
                    Color = Color.FromRgb(255, 255, 255),
                    Lifespan = 0.5 + rand.Next(200) / 1000d
                });
        }

        public void Update()
        {
            var updateInterval = .003;

            CreateParticle();

            particles.RemoveAll(p =>
            {
                p.Update(updateInterval);
                return p.Color.A == 0;
            });
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var canvas = new Canvas();

            Content = canvas;

            var emitter = new ParticleEmitter();

            var element = new DrawingVisualElement();



            CompositionTarget.Rendering += (s, e) =>
            {
                emitter.Update();

                canvas.Children.Clear();

                canvas.Children.Add(new Rectangle() { Fill = Brushes.DeepSkyBlue, Width = canvas.ActualWidth, Height = canvas.ActualHeight });

                using (var dc = element.visual.RenderOpen())
                {
                    //dc.DrawRectangle(
                    //    Brushes.Black,
                    //    null,
                    //    new Rect(
                    //        new Size(
                    //            canvas.ActualWidth,
                    //            canvas.ActualHeight)));

                    Random rand = new Random();
                    emitter.particles.ForEach(p =>
                    {

                        var c = p.Color;

                        c.A /= 2;
                        var diameter = rand.Next(0, 10);

                        dc.DrawEllipse(
                            new SolidColorBrush(c),
                            null, p.Position, 10, 10);

                        dc.DrawEllipse(
                            new SolidColorBrush(p.Color),
                            null, p.Position, 5, 5);
                    });
                }

                canvas.Children.Add(element);
            };

            MouseMove += (s, e) => emitter.Center = e.GetPosition(canvas);
        }
    }
    /*
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ParticleSystemManager _pm;
        private readonly Random _rand;
        private int _currentTick;
        private double _elapsed;
        private int _frameCount;
        private double _frameCountTime;
        private int _frameRate;
        private int _lastTick;
        private Point3D _spawnPoint;
        private double _totalElapsed;

        public MainWindow()
        {
            InitializeComponent();

            var frameTimer = new DispatcherTimer();
            frameTimer.Tick += OnFrame;
            frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            frameTimer.Start();

            _spawnPoint = new Point3D(0.0, 0.0, 0.0);
            _lastTick = Environment.TickCount;

            _pm = new ParticleSystemManager();

            WorldModels.Children.Add(_pm.CreateParticleSystem(1000, Colors.Gray));
            WorldModels.Children.Add(_pm.CreateParticleSystem(1000, Colors.Red));
            WorldModels.Children.Add(_pm.CreateParticleSystem(1000, Colors.Silver));
            WorldModels.Children.Add(_pm.CreateParticleSystem(1000, Colors.Orange));
            WorldModels.Children.Add(_pm.CreateParticleSystem(1000, Colors.Yellow));

            _rand = new Random(GetHashCode());

            KeyDown += Window1_KeyDown;
            Cursor = Cursors.None;
        }

        private void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void OnFrame(object sender, EventArgs e)
        {
            // Calculate frame time;
            _currentTick = Environment.TickCount;
            _elapsed = (_currentTick - _lastTick) / 1000.0;
            _totalElapsed += _elapsed;
            _lastTick = _currentTick;

            _frameCount++;
            _frameCountTime += _elapsed;
            if (_frameCountTime >= 1.0)
            {
                _frameCountTime -= 1.0;
                _frameRate = _frameCount;
                _frameCount = 0;
                FrameRateLabel.Content = "FPS: " + _frameRate + "  Particles: " + _pm.ActiveParticleCount;
            }

            _pm.Update((float)_elapsed);
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Red, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Orange, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Silver, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Gray, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Red, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Orange, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Silver, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Gray, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Red, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Yellow, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Silver, _rand.NextDouble(), 2.5 * _rand.NextDouble());
            _pm.SpawnParticle(_spawnPoint, 10.0, Colors.Yellow, _rand.NextDouble(), 2.5 * _rand.NextDouble());

            var c = Math.Cos(_totalElapsed);
            var s = Math.Sin(_totalElapsed);

            _spawnPoint = new Point3D(s * 32.0, c * 32.0, 0.0);
        }
    }
    */
}
