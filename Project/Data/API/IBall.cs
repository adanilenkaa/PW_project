using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Data.API
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get;}
        double Y { get;}
        double Rad { get; }
        void Move(double boardWidth, double boardHeight);
    }
}
