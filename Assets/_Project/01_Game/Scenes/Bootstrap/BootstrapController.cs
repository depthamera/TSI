using TSI.Core.Scene;
using UnityEngine;
using VContainer;

namespace TSI.Game
{
    public class BootstrapController : MonoBehaviour
    {
        [SerializeField] SceneProfile _initialSceneProfile;

        private ICustomSceneManager _sceneManager;

        [Inject]
        private void Construct(ICustomSceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        async void Start()
        {
            var root = _sceneManager.RegisterRootScene(gameObject.scene);
            await _sceneManager.LoadScene(_initialSceneProfile, root);
        }
    }
}
