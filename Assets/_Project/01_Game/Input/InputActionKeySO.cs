using UnityEngine;

namespace TSI.Game.Input
{
    /// <summary>
    /// 입력 액션 키. TimeLayerSO와 동일한 패턴 — SO 자체가 타입 안전한 키 역할.
    /// 에셋 예시: Move.asset, Attack.asset, Dash.asset
    /// </summary>
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_key.png")]
    [CreateAssetMenu(fileName = "ActionKey_New", menuName = "TSI/Input/Action Key")]
    public class InputActionKeySO : ScriptableObject { }
}
