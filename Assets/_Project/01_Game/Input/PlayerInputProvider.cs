using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TSI.Game.Input
{
    /// <summary>
    /// New Input System에서 입력을 읽는 IInputProvider 구현체.
    /// 인스펙터에서 InputActionKeySO ↔ InputActionReference 매핑을 편집.
    /// </summary>
    public class PlayerInputProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] private List<InputActionMapping> mappings = new();

        private Dictionary<InputActionKeySO, InputAction> _lookup;

        private void Awake()
        {
            _lookup = new Dictionary<InputActionKeySO, InputAction>();

            foreach (var m in mappings)
            {
                if (m.Key != null && m.Action != null && m.Action.action != null)
                    _lookup[m.Key] = m.Action.action;
            }
        }

        private void OnEnable()
        {
            foreach (var action in _lookup.Values)
                action.Enable();
        }

        private void OnDisable()
        {
            foreach (var action in _lookup.Values)
                action.Disable();
        }

        public Vector2 ReadAxis(InputActionKeySO key)
            => _lookup.TryGetValue(key, out var action) ? action.ReadValue<Vector2>() : Vector2.zero;

        public bool WasPressed(InputActionKeySO key)
            => _lookup.TryGetValue(key, out var action) && action.WasPressedThisFrame();

        public bool IsHeld(InputActionKeySO key)
            => _lookup.TryGetValue(key, out var action) && action.IsPressed();
    }
}
