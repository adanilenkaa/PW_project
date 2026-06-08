using Data.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Implementation
{
    internal class Board : API.DataApi
    {
        public override double BoardWidth => 800;
        public override double BoardHeight => 600;

        private readonly List<IBall> _balls = new List<IBall>();
        private readonly Queue<string> _logQueue = new Queue<string>();
        private readonly object _logLock = new object();
        private FileStream _logFile;
        private StreamWriter _logWriter;
        private bool _loggingActive = false;
        private Task _logWriterTask;
        private CancellationTokenSource _logCancellation;

        public override IBall CreateBall(double x, double y, double rad, double speedX, double speedY, double weight = 1.0)
        {
            var ball = new Ball(x, y, rad, speedX, speedY, weight);
            lock (_balls) { _balls.Add(ball); }
            return ball;
        }

        public override IEnumerable<IBall> GetBalls() { lock (_balls) return new List<IBall>(_balls); }
        public override void ClearBalls() { lock (_balls) _balls.Clear(); }

        public override void StartDiagnosticLogging(string filePath)
        {
            if (_loggingActive) return;

            _loggingActive = true;
            _logCancellation = new CancellationTokenSource();

            try
            {
                _logFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _logWriter = new StreamWriter(_logFile, Encoding.ASCII, bufferSize: 4096) { AutoFlush = false };
                _logWriter.WriteLine("DIAGNOSTIC LOG - Ball Simulation");
                _logWriter.WriteLine("Timestamp,BallId,X,Y,SpeedX,SpeedY,Rad,Weight");

                _logWriterTask = Task.Run(() => LogWriterLoop(_logCancellation.Token));
                Task.Run(() => LogTimerLoop(_logCancellation.Token));
            }
            catch (Exception)
            {
                _loggingActive = false;
                throw;
            }
        }

        public override void StopDiagnosticLogging()
        {
            if (!_loggingActive) return;
            _loggingActive = false;
            _logCancellation?.Cancel();

            try
            {
                if (_logWriterTask != null) _logWriterTask.Wait(TimeSpan.FromSeconds(5));

                lock (_logLock)
                {
                    while (_logQueue.Count > 0) _logWriter.WriteLine(_logQueue.Dequeue());
                }
                _logWriter?.Flush();
                _logWriter?.Close();
                _logFile?.Close();
            }
            catch { }
        }

        private async Task LogTimerLoop(CancellationToken token)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!token.IsCancellationRequested)
            {
                long timestamp = sw.ElapsedMilliseconds;
                foreach (var ball in GetBalls())
                {
                    string log = $"{timestamp},{ball.GetHashCode():X},{ball.X:F2},{ball.Y:F2},{ball.SpeedX:F2},{ball.SpeedY:F2},{ball.Rad:F2},{ball.Weight:F2}";
                    lock (_logLock) { _logQueue.Enqueue(log); }
                }
                await Task.Delay(100, token);
            }
        }

        private void LogWriterLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                    lock (_logLock)
                    {
                        while (_logQueue.Count > 0) _logWriter.WriteLine(_logQueue.Dequeue());
                        _logWriter.Flush();
                    }
                }
            }
            catch { }
        }
    }
}