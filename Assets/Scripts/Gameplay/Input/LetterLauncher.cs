using Core;
using DG.Tweening;
using Gameplay;
using Gameplay.Boxes;
using Gameplay.Letter;
using UnityEngine;

namespace Gameplay.Input
{
    /// <summary>
    /// Launches letters to slots based on slot index.
    /// Subscribes to SlotInputHandler for input.
    /// Does NOT know about SymbolType - only slot indices.
    /// </summary>
    public class LetterLauncher : MonoBehaviour
    {
        [SerializeField] private SlotInputHandler inputHandler;
        [SerializeField] private BoxSlotRegistry boxSlotRegistry;
        [SerializeField] private HitResolver hitResolver;
        [SerializeField] private LetterSpawner letterSpawner;
        [SerializeField] private float launchDuration = 0.3f;
        [SerializeField] private Ease launchEase = Ease.OutQuad;

        private Letter.Letter _currentLetter;
        private bool _isLaunching;
        private bool _inputLocked;
        private GameSessionController _session;

        private void Start()
        {
            if (inputHandler == null)
            {
                Debug.LogError($"[LetterLauncher] Missing required reference: inputHandler on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            if (boxSlotRegistry == null)
            {
                Debug.LogError($"[LetterLauncher] Missing required reference: boxSlotRegistry on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            if (hitResolver == null)
            {
                Debug.LogError($"[LetterLauncher] Missing required reference: hitResolver on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            if (letterSpawner == null)
            {
                Debug.LogError($"[LetterLauncher] Missing required reference: letterSpawner on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            _session = GameSessionController.Instance;
            if (_session == null)
            {
                Debug.LogError($"[LetterLauncher] GameSessionController.Instance is null in Start(). Disabling.");
                enabled = false;
                return;
            }

            inputHandler.OnSlotSelected += HandleSlotSelected;
            hitResolver.OnLetterResolved += HandleLetterResolved;
            letterSpawner.OnLetterSpawned += HandleLetterSpawned;
            letterSpawner.OnLetterCleared += HandleLetterCleared;
            _session.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (inputHandler != null)
            {
                inputHandler.OnSlotSelected -= HandleSlotSelected;
            }

            if (hitResolver != null)
            {
                hitResolver.OnLetterResolved -= HandleLetterResolved;
            }

            if (letterSpawner != null)
            {
                letterSpawner.OnLetterSpawned -= HandleLetterSpawned;
                letterSpawner.OnLetterCleared -= HandleLetterCleared;
            }

            if (_session != null)
            {
                _session.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState state)
        {
            if (state != GameSessionController.SessionState.Idle &&
                state != GameSessionController.SessionState.GameOver)
            {
                return;
            }
            KillCurrentTween();
            _currentLetter = null;
            _inputLocked = true;
            Debug.Log($"[LetterLauncher] State changed to {state}, input inactive");
        }

        private void HandleSlotSelected(int slotIndex)
        {
            if (_session.CurrentState != GameSessionController.SessionState.Playing)
            {
                return;
            }

            if (_inputLocked || _isLaunching) return;
            if (_currentLetter == null) return;

            LaunchToSlot(slotIndex);
        }

        private void LaunchToSlot(int slotIndex)
        {
            var targetBox = boxSlotRegistry.GetSlot(slotIndex);
            if (targetBox == null)
            {
                Debug.LogWarning($"[LetterLauncher] No box at slot {slotIndex}.");
                return;
            }

            _isLaunching = true;

            _currentLetter.transform
                .DOMove(targetBox.transform.position, launchDuration)
                .SetEase(launchEase)
                .OnComplete(() => _isLaunching = false);
        }

        private void HandleLetterResolved(LetterResolution resolution)
        {
            if (resolution.Letter != _currentLetter) return;

            _inputLocked = true;
            Debug.Log("[LetterLauncher] Letter resolved, input locked");
            KillCurrentTween();
            _currentLetter = null;
        }

        private void HandleLetterSpawned(Letter.Letter letter)
        {
            _currentLetter = letter;
            _inputLocked = false;
            Debug.Log($"[LetterLauncher] Letter spawned, input unlocked");
        }

        private void HandleLetterCleared()
        {
            KillCurrentTween();
            _currentLetter = null;
            _inputLocked = true;
            Debug.Log("[LetterLauncher] Letter cleared, input locked");
        }

        private void KillCurrentTween()
        {
            if (_currentLetter != null)
            {
                _currentLetter.transform.DOKill();
            }
            _isLaunching = false;
        }
    }
}
