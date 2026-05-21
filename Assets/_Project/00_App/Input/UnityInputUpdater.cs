using TSI.App.UI;
using TSI.Core.Time;
using UnityEngine.InputSystem;

namespace TSI.App.Input
{
    public class UnityInputUpdater : IInputUpdater
    {
        private readonly ManualEventSystem _eventSystem;

        public UnityInputUpdater(ManualEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }
        
        public void Update()
        {
            InputSystem.Update();
            _eventSystem.ManualUpdate();
        }
    }
}
