using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugTools
{
    public class DebugSessionControls : MonoBehaviour
    {
        [SerializeField] private InputActionAsset debugControls;

        private InputAction _startAction;
        private InputAction _endAction;
        private InputAction _restartAction;

        private void Awake()
        {
            var debugMap = debugControls.FindActionMap("Debug");
            _startAction = debugMap.FindAction("StartGame");
            _endAction = debugMap.FindAction("EndGame");
            _restartAction = debugMap.FindAction("RestartGame");

            _startAction.performed += _ => GameSessionController.Instance.StartGame();
            _endAction.performed += _ => GameSessionController.Instance.EndGame();
            _restartAction.performed += _ => GameSessionController.Instance.RestartGame();
        }

        private void OnEnable()
        {
            _startAction.Enable();
            _endAction.Enable();
            _restartAction.Enable();
        }

        private void OnDisable()
        {
            _startAction.Disable();
            _endAction.Disable();
            _restartAction.Disable();
        }
    }
}
