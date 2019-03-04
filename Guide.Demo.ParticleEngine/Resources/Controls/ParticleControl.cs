using Guide.Demo.ParticleEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Guide.Demo.ParticleEngine.Resources.Controls
{
    public class ParticleControl : Control
    {
        #region Properties

        #region Dependency Properties
        public ObservableCollection<Particle> Particles
        {
            get { return (ObservableCollection<Particle>)GetValue(ParticlesProperty); }
            set { SetValue(ParticlesProperty, value); }
        }
        public static readonly DependencyProperty ParticlesProperty =
            DependencyProperty.Register("Particles", typeof(ObservableCollection<Particle>),
                typeof(ParticleControl), new PropertyMetadata(new ObservableCollection<Particle>()));

        /*
        public Range SpawnX
        {
            get { return (Range)GetValue(SpawnXProperty); }
            set { SetValue(SpawnXProperty, value); }
        }
        public static readonly DependencyProperty SpawnXProperty =
            DependencyProperty.Register("SpawnX", typeof(Range), typeof(ParticleControl), new UIPropertyMetadata(new Range()));


        public Range SpawnY
        {
            get { return (Range)GetValue(SpawnYProperty); }
            set { SetValue(SpawnYProperty, value); }
        }

        public static readonly DependencyProperty SpawnYProperty =
            DependencyProperty.Register("SpawnY", typeof(Range), typeof(ParticleControl), new UIPropertyMetadata(new Range()));
        */

        public Color ParticleColor
        {
            get { return (Color)GetValue(ParticleColorProperty); }
            set { SetValue(ParticleColorProperty, value); }
        }
        public static readonly DependencyProperty ParticleColorProperty =
            DependencyProperty.Register("ParticleColor", typeof(Color), typeof(ParticleControl), new PropertyMetadata(System.Windows.Media.Colors.White));





        #endregion

        #region Internals
        DispatcherTimer Timer { get; } = new DispatcherTimer();
        Random Random { get; } = new Random();
        Range SpawnX { get; set; } = new Range() { Start = 0, End = 500 };
        Range SpawnY { get; set; } = new Range() { Start = 500, End = 500 };
        #endregion

        #endregion

        #region Constructors
        static ParticleControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ParticleControl), new FrameworkPropertyMetadata(typeof(ParticleControl)));
        }

        public ParticleControl()
        {
            Loaded += (s, e) => Initialize();
        }
        #endregion

        #region Methods

        void Initialize()
        {
            for (int i = 0; i < 200; i++)
                CreateParticle();

            CompositionTarget.Rendering += (s, e) =>
            {
                
                for (int i = 0; i < Particles.Count; i++)
                    Particles[i].Update();
            };
            /*
            Timer.Interval = TimeSpan.FromMilliseconds(20);

            Timer.Tick += (s, e) =>
            {
                for (int i = 0; i < Particles.Count; i++)
                    Particles[i].Update();
            };
            Timer.Start();
            */
        }

        void CreateParticle()
        {
            double x = RandomDouble(Random, SpawnX.Start, SpawnX.End);
            double y = RandomDouble(Random, SpawnY.Start, SpawnY.End);
            Particle particle = null;
            Particles.Add(particle = new Particle
            {
                Position = new Point(x, y),
                Size = new Size(10, 10),
                Color = ParticleColor,
            });

            EventHandler withered = null;
            particle.Withered += withered = (s, e) =>
            {
                particle.Withered -= withered;
                Particles.Remove(particle);

                CreateParticle();
            };
        }

        static double RandomDouble(Random random, double minimum, double maximum)
        {
            random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

    #region Overrides

    #endregion

    #endregion
}
}
