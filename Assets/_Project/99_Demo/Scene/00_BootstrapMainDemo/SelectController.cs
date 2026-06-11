using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using TSI.Core.Scene;
using UnityEngine;
using UnityEngine.UI;
using VContainer;


namespace TSI.Demo
{
    public struct GameStartOption
    {
        public string CharacterName;
    }

    public class SelectController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _optionsDropdown;
        [SerializeField] private Button _startButton;
        [SerializeField] private SceneProfile _gameProfile;
        private GameStartOption _startInfo;

        private void Awake()
        {
            _optionsDropdown.onValueChanged.AsObservable()
                .Prepend(_optionsDropdown.value) 
                .Subscribe(ChangeOption)
                .AddTo(destroyCancellationToken);
        }

        [Inject]
        private void Construct(ICustomSceneManager sceneManager)
        {
            var handle = sceneManager.Resolve(gameObject.scene);
            var parentHandle = sceneManager.GetParent(handle);

            _startButton.OnClickAsObservable()
                .Subscribe(_ => sceneManager.ReplaceScene(_gameProfile, parentHandle, default, builder =>
                {
                    builder.RegisterInstance(_startInfo);
                }))
                .AddTo(destroyCancellationToken);
        }

        private void ChangeOption(int index)
        {
            _startInfo.CharacterName = _optionsDropdown.options[index].text;
            Debug.Log($"선택된 캐릭터: {_startInfo.CharacterName}");
        }
    }
}
