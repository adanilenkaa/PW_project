using System.Collections.Generic;

namespace Data.API
{
    public abstract class DataApi
    {
        public abstract double BoardWidth { get; }
        public abstract double BoardHeight { get; }
        public abstract IBall CreateBall(double x, double y, double rad, double speedX, double speedY, double weight = 1.0);
        public abstract IEnumerable<IBall> GetBalls();
        public abstract void ClearBalls();

        public abstract void StartDiagnosticLogging(string filePath);
        public abstract void StopDiagnosticLogging();

        public static DataApi Create()
        {
            return new Implementation.Board();
        }
    }
}