using UnityEngine;

namespace TSI.Game.Input
{
    /// <summary>
    /// 입력 소스 추상화. 플레이어(New Input System)와 AI(프로그래밍 값)가 같은 인터페이스를 구현.
    /// 액션 추가 시 인터페이스 변경 불필요 — InputActionKeySO 에셋만 추가하면 됨.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        /// Vector2 값 읽기 (이동 등). 매핑 없는 키는 Vector2.zero 반환.
        /// </summary>
        Vector2 ReadAxis(InputActionKeySO key);

        /// <summary>
        /// 이번 프레임에 눌렸는가 (원샷). 매핑 없는 키는 false 반환.
        /// </summary>
        bool WasPressed(InputActionKeySO key);

        /// <summary>
        /// 현재 눌려 있는가 (지속). 매핑 없는 키는 false 반환.
        /// </summary>
        bool IsHeld(InputActionKeySO key);
    }
}
