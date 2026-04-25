using Data.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Data.Implementation
{
    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        public double Rad { get;}

        private double _speedX;
        private double _speedY;

        public event PropertyChangedEventHandler PropertyChanged;

        public Ball(double x, double y, double rad, double speedX, double speedY)
        {
            _x = x;
            _y = y;
            Rad = rad;
            _speedX = speedX;
            _speedY = speedY;
        }

        public double X
        {
            get => _x;
            private set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            private set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Move(double boardWidth, double boardHeight)
        {
            double newX = X + _speedX;
            double newY = Y + _speedY;
            if (newX - Rad < 0 || newX + Rad > boardWidth)
            {
                _speedX = -_speedX;
                newX = X + _speedX;
            }
            if (newY - Rad < 0 || newY + Rad > boardHeight)
            {
                _speedY = -_speedY;
                newY = Y + _speedY;
            }
            X = newX;
            Y = newY;
        }
    }
}
