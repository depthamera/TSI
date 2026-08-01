using MessagePipe;
using UnityEngine.InputSystem;

namespace TSI.Core.Time
{
    
    public class InputClock : ClockBase
    {

        public InputClock(IPublisher<TimeLayerSO, TickMessage> publisher, TimeLayerSO layer, IPublisher<TimeLayerSO, TimeStateMessage> statePublisher) : base(publisher, layer, statePublisher) { }

        internal override void Tick(float deltaTime)
        {
            InputSystem.Update();
        }
    }
}