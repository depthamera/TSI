using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace TSI.Demo
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private Button _testButton;

        [Inject]
        private void Construct(GlobalSystemNotifier notifier)
        {
            notifier.ShowMessage("Bootstrap->Main");

            _testButton.OnClickAsObservable()
                .Subscribe(e => notifier.ShowMessage("버튼 클릭됨"))
                .AddTo(destroyCancellationToken);
        }
    }
}
