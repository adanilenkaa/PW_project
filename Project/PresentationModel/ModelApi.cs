using Logic.Api;
using System.Collections.Generic;

namespace Presentation.Model
{
    public abstract class ModelApi
    {
        public abstract void Start(int ballCount);
        public abstract void Stop();
        public abstract IEnumerable<object> GetBalls();

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

        public override IEnumerable<object> GetBalls() => _logicApi.GetBalls();
    }
}