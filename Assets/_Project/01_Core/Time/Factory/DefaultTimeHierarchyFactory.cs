using System.Collections.Generic;
using MessagePipe;

namespace TSI.Core.Time
{
    public class DefaultTimeHierarchyFactory : ITimeHierarchyFactory
    {
        private readonly IPublisher<TimeLayer, TickMessage> _publisher;
        private readonly IInputUpdater _inputUpdater;

        public DefaultTimeHierarchyFactory(
            IPublisher<TimeLayer, TickMessage> publisher,
            IInputUpdater inputUpdater)
        {
            _publisher = publisher;
            _inputUpdater = inputUpdater;
        }
        
        public TimeHierarchyResult Create()
        {
            var builder = new TimeHierarchyBuilder();

            //var inputClock = new InputClock(_inputUpdater);
            //builder.RegisterRoot(inputClock);
            
            //var globalClock = new ContinuousClock(_publisher, TimeLayer.Global);
            //builder.RegisterRoot(globalClock);

            //var lateClock = new ContinuousClock(_publisher, TimeLayer.Late);
            //builder.RegisterRoot(lateClock);
            
            //var physicsClock = new PhysicsClock(_publisher, TimeLayer.Physics);
            //builder.RegisterChild(globalClock, physicsClock);

            //var gameClock = new ContinuousClock(_publisher, TimeLayer.Gameplay);
            //builder.RegisterChild(globalClock, gameClock);

            //var uiClock = new ContinuousClock(_publisher, TimeLayer.UI);
            //builder.RegisterChild(globalClock, uiClock);

            return builder.Build();
        }
    }
}