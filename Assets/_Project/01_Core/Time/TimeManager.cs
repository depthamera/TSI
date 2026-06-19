using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace TSI.Core.Time
{
    public class TimeManager : ITickable
    {
        private const float MaxDeltaTime = 0.1f;
        
        private readonly List<ClockBase> _rootClocks;
        private readonly Dictionary<TimeLayerSO, IClock> _clocks;

        public TimeManager(ITimeHierarchyFactory factory)
        {
            var result = factory.Create();

            _rootClocks = result.RootClocks;
            _clocks = result.Clocks;
        }
        
        public T GetClock<T>(TimeLayerSO layer) where T : class, IClock
        {
            var clock = _clocks.GetValueOrDefault(layer);
            return clock as T
                   ?? throw new InvalidOperationException($"{layer} is not a {typeof(T).Name}.");
        }

        public IClock GetClock(TimeLayerSO layer)
        {
            return _clocks.GetValueOrDefault(layer);
        }
        
        public IFixedClock GetFixedClock(TimeLayerSO layer)
        {
            var clock = _clocks.GetValueOrDefault(layer);
            return clock as IFixedClock
                   ?? throw new InvalidOperationException($"{layer} is not a FixedClock.");
        }

        public void Tick()
        {
            var deltaTime = Mathf.Min(UnityEngine.Time.unscaledDeltaTime, MaxDeltaTime);
            
            foreach (var rootClock in _rootClocks)
                rootClock.Tick(deltaTime);
        }
    }
}