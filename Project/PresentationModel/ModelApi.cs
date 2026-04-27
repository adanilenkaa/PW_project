using Logic.Api;
using System.Collections.Generic;

namespace PresentationModel
{
    public abstract class ModelApi
    {
        public abstract void Start(int ballCount);
        public abstract void Stop();
        public abstract IEnumerable<BallModel> GetBalls();

        public static ModelApi CreateApi(LogicApi logicApi = null)
        {
            return new ModelImplementation(logicApi ?? LogicApi.CreateApi());
        }
    }

    internal class ModelImplementation : ModelApi
    {
        private readonly LogicApi _logicApi;
        public ModelImplementation(LogicApi logicApi)
        {
            _logicApi = logicApi;
        }

        public override void Start(int ballCount)
        {
            _logicApi.GenerateBalls(ballCount);
            _logicApi.Start();
        }

        public override void Stop() => _logicApi.Stop();

        public override IEnumerable<BallModel> GetBalls()
        {
            var visualBalls = new List<BallModel>();
            foreach (var ball in _logicApi.GetBalls())
            {
                visualBalls.Add(new BallModel(ball)); 
            }
            return visualBalls;
        }
    }
}