namespace TSI.Core.Time
{
    public readonly struct TickMessage
    {
        public readonly float DeltaTime;
        public TickMessage(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}