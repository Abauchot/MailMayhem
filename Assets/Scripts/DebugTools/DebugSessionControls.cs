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
            if (debugControls == null)
            {
                Debug.LogError("DebugControls asset is not assigned in DebugSessionControls!");
                return;
            }

            var debugMap = debugControls.FindActionMap("Debug");
            if (debugMap == null)
            {
                Debug.LogError("Debug action map not found in DebugControls asset!");
                return;
            }

            _startAction = debugMap.FindAction("StartGame");
            _endAction = debugMap.FindAction("EndGame");
            _restartAction = debugMap.FindAction("RestartGame");

            if (_startAction == null || _endAction == null || _restartAction == null)
            {
                Debug.LogError("One or more actions not found in Debug action map!");
                return;
            }

            _startAction.performed += _ => 
            {
                Debug.Log("Debug: StartGame action performed");
                GameSessionController.Instance?.StartGame();
            };
            
            _endAction.performed += _ => 
            {
                Debug.Log("Debug: EndGame action performed");
                GameSessionController.Instance?.EndGame();
            };

            _restartAction.performed += _ => 
            {
                Debug.Log("Debug: RestartGame action performed");
                GameSessionController.Instance?.RestartGame();
            };
        }

        private void OnEnable()
        {
            _startAction?.Enable();
            _endAction?.Enable();
            _restartAction?.Enable();
        }

        private void OnDisable()
        {
            _startAction?.Disable();
            _endAction?.Disable();
            _restartAction?.Disable();
        }
    }
}
