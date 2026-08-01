using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TSI.Game.Input
{
    /// <summary>
    /// InputActionKeySO ↔ New Input System InputActionReference 매핑 항목.
    /// PlayerInputProvider의 매핑 리스트에 사용.
    /// </summary>
    [Serializable]
    public class InputActionMapping
    {
        public InputActionKeySO Key;
        public InputActionReference Action;
    }
}
