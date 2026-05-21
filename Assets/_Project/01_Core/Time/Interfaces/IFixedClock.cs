namespace TSI.Core.Time
{
    public interface IFixedClock : IClock
    {
        public float FixedTimestep { get; set; }
        public int MaxSteps { get; set; }
    }
}