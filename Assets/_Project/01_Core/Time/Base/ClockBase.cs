using System.Collections.Generic;
using MessagePipe;

namespace TSI.Core.Time
{
    public abstract class ClockBase : IClock
    {
        protected IPublisher<TimeLayer, TickMessage> Publisher { get; }
        protected List<ClockBase> Children { get; } = new();
        
        public TimeLayer Layer { get; }
        public float DeltaTime { get; protected set; }
        public float TimeScale { get; set; } = 1;
        public bool IsPaused { get; private set; }

        protected ClockBase(IPublisher<TimeLayer, TickMessage> publisher, TimeLayer layer)
        {
            Publisher = publisher;
            Layer = layer;
        }

        public void AddChild(ClockBase childClock)
        {
            Children.Add(childClock);
        }
        
        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
        
        internal abstract void Tick(float deltaTime);
    }
}