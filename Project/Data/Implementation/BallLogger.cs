using Data.API;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Implementation
{
    internal class BallLogger
    {
        private readonly string _filePath;
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _loggingTask;

        public BallLogger(string filePath = "billiard_diagnostics.json")
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            // Czyszczenie pliku przy uruchomieniu nowej sesji
            if (File.Exists(_filePath)) File.Delete(_filePath);

            // Wątek konsumenta - działa w tle, wyciąga dane z bufora i pisze na dysk
            _loggingTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested || !_logQueue.IsEmpty)
                {
                    if (_logQueue.TryDequeue(out string logEntry))
                    {
                        try
                        {
                            using (StreamWriter sw = File.AppendText(_filePath))
                            {
                                await sw.WriteLineAsync(logEntry);
                            }
                        }
                        catch (IOException)
                        {
                            
                            _logQueue.Enqueue(logEntry);
                            await Task.Delay(10);
                        }
                    }
                    else
                    {
                        await Task.Delay(50); 
                    }
                }
            });
        }

        public void LogState(IEnumerable<IBall> balls)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var snapShotList = new List<object>();

            foreach (var ball in balls)
            {
                snapShotList.Add(new
                {
                    X = Math.Round(ball.X, 2),
                    Y = Math.Round(ball.Y, 2),
                    Vx = Math.Round(ball.SpeedX, 2),
                    Vy = Math.Round(ball.SpeedY, 2)
                });
            }

            var logObject = new
            {
                Timestamp = timestamp,
                Balls = snapShotList
            };

            
            string jsonString = JsonSerializer.Serialize(logObject);
            _logQueue.Enqueue(jsonString); 
        }

        public void Stop()
        {
            _cts.Cancel();
            _loggingTask.Wait();
        }
    }
}