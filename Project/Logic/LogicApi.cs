using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class LogicApi
    {
        public List<Ball> CreateBalls()
        {
            List<Ball> balls = new List<Ball>();
            balls.Add(new Ball(10, 10));
            balls.Add(new Ball(50, 50));
            return balls;
        }
    }
}
