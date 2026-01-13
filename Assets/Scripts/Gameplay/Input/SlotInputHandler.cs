using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Input;

namespace Gameplay.Input
{
    /// <summary>
    /// Translates player input to slot indices (0-3).
    /// Does NOT know about SymbolType, boxes, or game state.
    /// Slot index represents a stable lane position.
    /// </summary>
    public class SlotInputHandler : MonoBehaviour
    {
        private InputSystem_Actions _inputs;
        private bool _bound;

        /// <summary>
        /// Fired when player selects a slot. Parameter is slotIndex (0-3).
        /// </summary>
        public event Action<int> OnSlotSelected;

        private void EnsureInputs()
        {
            _inputs ??= new InputSystem_Actions();
        }

        private void OnEnable()
        {
            EnsureInputs();
            _inputs.Player.Enable();

            if (_bound) return;
            _bound = true;

            // Mapping: Q=slot0, W=slot1, E=slot2, R=slot3 (QWERTY order)
            _inputs.Player.Throw_Up.performed += OnSlot0;      // Q key
            _inputs.Player.Throw_Down.performed += OnSlot1;    // W key
            _inputs.Player.Throw_Left.performed += OnSlot2;    // E key
            _inputs.Player.Throw_Right.performed += OnSlot3;   // R key
        }

        private void OnDisable()
        {
            if (_inputs == null) return;

            if (_bound)
            {
                _inputs.Player.Throw_Up.performed -= OnSlot0;
                _inputs.Player.Throw_Down.performed -= OnSlot1;
                _inputs.Player.Throw_Left.performed -= OnSlot2;
                _inputs.Player.Throw_Right.performed -= OnSlot3;
                _bound = false;
            }

            _inputs.Player.Disable();
        }

        private void OnDestroy()
        {
            _inputs?.Dispose();
            _inputs = null;
        }

        // Slot mapping: Q=0, W=1, E=2, R=3 (QWERTY/AZERTY order)
        private void OnSlot0(InputAction.CallbackContext _) => OnSlotSelected?.Invoke(0);
        private void OnSlot1(InputAction.CallbackContext _) => OnSlotSelected?.Invoke(1);
        private void OnSlot2(InputAction.CallbackContext _) => OnSlotSelected?.Invoke(2);
        private void OnSlot3(InputAction.CallbackContext _) => OnSlotSelected?.Invoke(3);
    }
}
