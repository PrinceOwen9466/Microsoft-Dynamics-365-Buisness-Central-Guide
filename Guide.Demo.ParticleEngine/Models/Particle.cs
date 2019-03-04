using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Guide.Demo.ParticleEngine.Models
{
    public class Particle : BindableBase
    {
        #region Properties

        #region Bindables
        Point _position = new Point(0, 0);
        public Point Position { get => _position; set => SetProperty(ref _position, value); }

        Size _size = new Size(10, 10);
        public Size Size { get => _size; set => SetProperty(ref _size, value); }

        Point _velocity = new Point(150, 150);
        public Point Velocity { get => _velocity; set => SetProperty(ref _velocity, value); }

        Color _color = Colors.White;
        public Color Color { get => _color; set => SetProperty(ref _color, value); }

        Rect _bounds = new Rect(new Size(1000, 1000));
        public Rect Bounds { get => _bounds; set => SetProperty(ref _bounds, value); }

        public event EventHandler Withered;
        #endregion

        #region Internals
        DateTime Created { get; } = DateTime.Now;
        Random Random { get; } = new Random();
        #endregion

        #endregion


        #region Constructors
        public Particle() { }
        public Particle(double x, double y) : this(x, y, 150, 150, 10, Colors.White) { }
        public Particle(double x, double y, double radius, Color color) : this(x, y, 150, 150, radius, color) { }

        public Particle(double x, double y, double velocityX, double velocityY, double radius, Color color)
        {
            Position = new Point(x, y);
            Velocity = new Point(velocityX, velocityY);
            Color = color;
            Size = new Size(radius*2, radius*2);

            CompositionTarget.Rendering += (s, e) =>
            {
                var elapsedTime = DateTime.Now - Created;
                Position = new Point(Position.X + Random.Next(-1, 1), Position.Y - 1);
                RaisePropertyChanged(nameof(Position));

                if (!Bounds.Contains(new Point(Position.X + 50, Position.Y + 50)))
                    Withered?.Invoke(this, EventArgs.Empty);
            };
        }
        #endregion

        #region Methods
        public void Update()
        {
            var elapsedTime = DateTime.Now - Created;
            Position = new Point(Position.X + Random.Next(-1, 1), Position.Y - 1);
            RaisePropertyChanged(nameof(Position));

            if (!Bounds.Contains(new Point(Position.X + 50, Position.Y + 50)))
                Withered?.Invoke(this, EventArgs.Empty);

        }
        #endregion
    }
}
