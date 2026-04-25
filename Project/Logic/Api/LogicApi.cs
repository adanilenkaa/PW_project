using System;
using System.Collections.Generic;
using System.Text;
using Data.API;
using Logic.Implementation;

namespace Logic.Api
{
    internal abstract class LogicApi
    {
        public abstract void GenerateBalls(int count);

        public abstract void Start();
        public abstract void Stop();

        public abstract IEnumerable<IBall> GetBalls();
        public abstract double GetBoardWidth();
        public abstract double GetBoardHeight();

        public static LogicApi CreateApi(DataApi dataApi = null)
        {
            return new Simulation(dataApi ?? DataApi.Create());

        }

    }
}

