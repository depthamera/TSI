using UnityEngine;

namespace TSI.Game.FSM
{
    [Icon("Assets/_Project/02_Editor/Icons/icon_so_config.png")]
    [CreateAssetMenu(fileName = "Stats_New", menuName = "TSI/FSM/Agent Stats")]
    public class AgentStatsSO : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 5f;

        public float BaseSpeed => baseSpeed;

        /// <summary>
        /// SO 값을 복사한 런타임 인스턴스 생성.
        /// 인스턴스별 버프/디버프가 SO 원본을 오염시키지 않도록 분리.
        /// </summary>
        public AgentStats CreateRuntime() => new(this);
    }
}
