using TSI.Core.Scene;
using UnityEngine;
using VContainer;

namespace TSI.Demo
{
    public class BootstrapController : MonoBehaviour
    {
        [SerializeField] private SceneProfile _mainProfile;
        [SerializeField] private SceneProfile _loadingProfile;
        private ICustomSceneManager _sceneManager;

        [Inject]
        public void Construct(ICustomSceneManager sceneManager, GlobalSystemNotifier notifier)
        {
            _sceneManager = sceneManager;
            _sceneManager.RegisterRootScene(gameObject.scene);

            notifier.ShowMessage("Bootstrap에서 등록됨");
        }

        private async void Start()
        {
            var currentScene = _sceneManager.Resolve(gameObject.scene);
            await _sceneManager.LoadScene(_mainProfile, currentScene, new SceneTransitionOptions(_loadingProfile, 1f));
        }
    }
}
