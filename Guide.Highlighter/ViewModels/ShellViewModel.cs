using Guide.Highlighter.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Guide.Highlighter.Extensions.Win32;

namespace Guide.Highlighter.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Properties

        #region Bindables
        double _mouseX = -100;
        public double MouseX { get => _mouseX; set => SetProperty(ref _mouseX, value); }

        double _mouseY = -100;
        public double MouseY { get => _mouseY; set => SetProperty(ref _mouseY, value); }

        MouseMessages _mouseMessage = MouseMessages.None;
        public MouseMessages MouseMessage { get => _mouseMessage; set => SetProperty(ref _mouseMessage, value); }

        bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                bool _wasActive = _isActive;
                SetProperty(ref _isActive, value);

                if (_isActive && !_wasActive)
                    Win32.SetMouseHook();
                else if (!_isActive) Win32.RemoveMouseHook();
            }
        }
        #endregion

        #region Commands
        public ICommand ToggleActiveCommand { get; }
        public ICommand CloseCommand { get; }
        #endregion

        #endregion

        #region Constructors

        public ShellViewModel()
        {
            ToggleActiveCommand = new DelegateCommand(OnToggleActive);
            CloseCommand = new DelegateCommand(OnClose);

            Win32.SetMouseHook();
            Win32.MouseAction += async (s, ex) =>
            {
                MouseX = ex.Position.X - 10;
                MouseY = ex.Position.Y - 8;

                if (ex.Message != MouseMessages.WM_MOUSEMOVE)
                {
                    if (ex.Message == MouseMessages.WM_LBUTTONUP)
                        await Task.Delay(200);
                    MouseMessage = ex.Message;
                }
                Console.WriteLine("({0}, {1}) - {2}", MouseX, MouseY, MouseMessage);
            };

            /*
            Point point = Mouse.GetPosition(Application.Current.MainWindow);
            MouseX = point.X - 15;
            MouseY = point.Y - 10;
            */
        }

        #endregion

        #region Methods

        #region Command Handlers

        
        void OnToggleActive() => IsActive = !IsActive;

        void OnClose() => Application.Current.Dispatcher.InvokeShutdown();

        #endregion

        #endregion
    }
}
