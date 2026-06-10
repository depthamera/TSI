using UnityEngine;

namespace TSI.Demo
{
    public class GlobalSystemNotifier
    {
        public void ShowMessage(string message)
        {
            Debug.Log($"<color=cyan>[Boostrap]</color> {message}");
        }
    }
}
