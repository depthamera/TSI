using UnityEngine.EventSystems;

namespace TSI.App.UI
{
    public class ManualEventSystem : EventSystem
    {
        protected override void Update() { }

        public void ManualUpdate()
        {
            base.Update();
        }
    }
}
