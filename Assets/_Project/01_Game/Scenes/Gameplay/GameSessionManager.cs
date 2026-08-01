using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using TSI.Core.Scene;
using VContainer.Unity;

namespace TSI.Game
{
    public class GameSessionManager : IAsyncStartable, IDisposable
    {

        private readonly GameSessionOptions _options;
        private readonly ICustomSceneManager _sceneManager;
        private readonly SceneHandle _gameplayHandle;

        private readonly DisposableBag _disposables;

        private SceneHandle _currentStageHandle;

        public GameSessionManager(GameSessionOptions options, ICustomSceneManager sceneManager)
        {
            _options = options;
            _sceneManager = sceneManager;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            _currentStageHandle = await _sceneManager.LoadScene(_options.InitialStage, _gameplayHandle);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
