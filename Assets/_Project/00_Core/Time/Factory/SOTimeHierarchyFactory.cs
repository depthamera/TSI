using MessagePipe;
using System.Collections.Generic;
using TSI.Core.Time;

namespace TSI.Core
{
    public class SOTimeHierarchyFactory : ITimeHierarchyFactory
    {
        private readonly TimeHierarchyProfileSO _profile;
        private readonly IPublisher<TimeLayerSO, TickMessage> _publisher;
        private readonly IPublisher<PhysicsInterpolationMessage> _intertpolationPublisher;
        private readonly IPublisher<TimeLayerSO, TimeStateMessage> _statePublisher;
        private readonly PhysicsInterpolationManager _interpolationManager;

        public SOTimeHierarchyFactory
            (TimeHierarchyProfileSO profile, IPublisher<TimeLayerSO, TickMessage> publisher,
            IPublisher<PhysicsInterpolationMessage> interpolationPublisher,
            IPublisher<TimeLayerSO, TimeStateMessage> statePublisher,
            PhysicsInterpolationManager interpolationManager)
        {
            _profile = profile;
            _publisher = publisher;
            _intertpolationPublisher = interpolationPublisher;
            _statePublisher = statePublisher;
            _interpolationManager = interpolationManager;
        }

        public TimeHierarchyResult Create()
        {
            var builder = new TimeHierarchyBuilder();
            CreateClocks(builder);

            return builder.Build();
        }

        private void CreateClocks(TimeHierarchyBuilder builder)
        {
            // 순회하며 stack을 이용해 자신의 parent를 찾아 등록하도록 변경.
            Stack<(ClockBase clock, int depth)> parentClocks = new();

            foreach (var node in _profile.Nodes) 
            {
                ClockBase clock = node.ClockType switch
                {
                    ClockType.Continuous => new ContinuousClock(_publisher, node.LayerKey, _statePublisher),
                    ClockType.Fixed => new FixedClock(_publisher, node.LayerKey, _statePublisher),
                    ClockType.Physics => new PhysicsClock(_publisher, node.LayerKey, _intertpolationPublisher, _statePublisher,
                        _interpolationManager.RestorePositions),
                    ClockType.Input => new InputClock(_publisher, node.LayerKey, _statePublisher),
                    _ => throw new System.NotImplementedException(),
                };

                while(parentClocks.TryPeek(out var data) && data.depth >= node.Depth)
                    parentClocks.Pop();

                if(parentClocks.TryPeek(out var parent))
                    builder.RegisterChild(parent.clock, clock);
                else 
                    builder.RegisterRoot(clock);

                parentClocks.Push((clock, node.Depth));
            }          
            
        }
    }
}
