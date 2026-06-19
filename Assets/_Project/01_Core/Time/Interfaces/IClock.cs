namespace TSI.Core.Time
{
    public interface IClock
    {
        TimeLayerSO Layer { get; }
        float DeltaTime { get; }
        float TimeScale { get; set; }
        bool IsPaused { get; }
        
        void Pause();
        void Resume();
        
    }
}