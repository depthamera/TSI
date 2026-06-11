using UnityEngine;
using VContainer;

namespace TSI.Demo
{
    public class GameController : MonoBehaviour
    {
        [Inject]
        private void Constrcut(GameStartOption option)
        {
            Debug.Log($"게임 시작! 선택된 캐릭터: {option.CharacterName}");
        }
    }
}
