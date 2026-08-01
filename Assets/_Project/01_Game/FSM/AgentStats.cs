namespace TSI.Game.FSM
{
    /// <summary>
    /// AgentStatsSO의 런타임 복사본.
    /// Blackboard core 필드로 보유하며, 인스턴스별 버프/디버프 적용 가능.
    /// </summary>
    public class AgentStats
    {
        public float BaseSpeed { get; set; }

        public AgentStats(AgentStatsSO source)
        {
            BaseSpeed = source.BaseSpeed;
        }
    }
}
