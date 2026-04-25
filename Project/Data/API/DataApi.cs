using Data.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.API
{
    public abstract class DataApi
    {
        public abstract double BoardWidth { get; }
        public abstract double BoardHeight { get; }
        public abstract IBall CreateBall(double x, double y, double rad, double speedX, double speedY);
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void ClearBalls();

        public static DataApi Create()
        {
            return new Board();
        }

    }
}
