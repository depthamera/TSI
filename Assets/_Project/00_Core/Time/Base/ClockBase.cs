using System.Collections.Generic;
using MessagePipe;

namespace TSI.Core.Time
{
    public abstract class ClockBase : IClock
    {
        protected IPublisher<TimeLayerSO, TickMessage> TickPublisher { get; }
        private IPublisher<TimeLayerSO, TimeStateMessage> TimeStatePublisher { get; }
        protected List<ClockBase> Children { get; } = new();
        
        public TimeLayerSO Layer { get; }
        public float DeltaTime { get; protected set; }
        public float TimeScale { get; set; } = 1;
        public bool IsPaused { get; private set; }

        protected ClockBase(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer, IPublisher<TimeLayerSO, TimeStateMessage> statePublisher)
        {
            TickPublisher = publisher;
            Layer = layer;
            TimeStatePublisher = statePublisher;
        }

        public void AddChild(ClockBase childClock)
        {
            Children.Add(childClock);
        }
        
        public void Pause()
        {
            IsPaused = true;
            TimeStatePublisher.Publish(Layer, new(true));

            foreach (IClock childClock in Children)      
                childClock.Pause();
            
        }

        public void Resume()
        {
            IsPaused = false;
            TimeStatePublisher.Publish(Layer, new(false));

            foreach (IClock childClock in Children)
                childClock.Resume();
        }
        
        internal abstract void Tick(float deltaTime);
    }
}