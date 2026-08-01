namespace TSI.Core.Time
{
    public readonly struct TimeStateMessage
    {
        public readonly bool IsPaused;
        public TimeStateMessage(bool isPaused) => IsPaused = isPaused;
    }
}
