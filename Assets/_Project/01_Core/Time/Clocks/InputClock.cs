using MessagePipe;
using UnityEngine.InputSystem;

namespace TSI.Core.Time
{
    
    public class InputClock : ClockBase
    {
        private readonly IInputUpdater _inputUpdater;
        
        public InputClock(IInputUpdater inputUpdater)
            : base(null, null)
        {
            _inputUpdater = inputUpdater;
        }

        internal override void Tick(float deltaTime)
        {
            _inputUpdater.Update();
        }
    }
}