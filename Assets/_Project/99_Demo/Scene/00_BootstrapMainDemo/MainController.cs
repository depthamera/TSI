using Cysharp.Threading.Tasks;
using R3;
using System;
using TSI.Core.Scene;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace TSI.Demo
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private Button _messageButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private SceneProfile _selectProfile;

        [Inject]
        private void Construct(GlobalSystemNotifier notifier, ICustomSceneManager sceneManager)
        {
            notifier.ShowMessage("Bootstrap->Main");

            _messageButton.OnClickAsObservable()
                .Subscribe(_ => notifier.ShowMessage("버튼 클릭됨"))
                .AddTo(destroyCancellationToken);

            var handle = sceneManager.Resolve(gameObject.scene);
            _selectButton.OnClickAsObservable()
                .Subscribe(_ => sceneManager.LoadScene(_selectProfile, handle))
                .AddTo(destroyCancellationToken);
        }
    }
}
