using Cysharp.Threading.Tasks;
using TSI.Core.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace TSI.Demo
{
    public class TestLoadingBar : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private Slider _slider;

        public UniTask Hide()
        {
            return UniTask.CompletedTask;
        }

        public void SetProgress(float progress)
        {
            _slider.value = progress;
        }

        public UniTask Show()
        {
            return UniTask.CompletedTask;
        }
    }
}
