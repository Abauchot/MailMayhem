using Core;
using DG.Tweening;
using Gameplay.Boxes;
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
        [SerializeField] private float launchDuration = 0.3f;
        [SerializeField] private Ease launchEase = Ease.OutQuad;

        private Letter.Letter _currentLetter;
        private bool _isLaunching;

        private void OnEnable()
        {
            if (inputHandler != null)
            {
                inputHandler.OnSlotSelected += HandleSlotSelected;
            }

            if (hitResolver != null)
            {
                hitResolver.OnLetterResolved += HandleLetterResolved;
            }

            if (GameSessionController.Instance != null)
            {
                GameSessionController.Instance.OnStateChanged += HandleStateChanged;
            }
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

            if (GameSessionController.Instance != null)
            {
                GameSessionController.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState state)
        {
            if (state == GameSessionController.SessionState.Idle || state == GameSessionController.SessionState.GameOver)
            {
                KillCurrentTween();
                _currentLetter = null;
            }
        }

        private void HandleSlotSelected(int slotIndex)
        {
            if (GameSessionController.Instance == null ||
                GameSessionController.Instance.CurrentState != GameSessionController.SessionState.Playing)
            {
                return;
            }

            if (_isLaunching) return;

            TryFindCurrentLetter();
            if (_currentLetter == null) return;

            LaunchToSlot(slotIndex);
        }

        private void TryFindCurrentLetter()
        {
            if (_currentLetter != null) return;
            _currentLetter = FindFirstObjectByType<Letter.Letter>();
        }

        private void LaunchToSlot(int slotIndex)
        {
            if (boxSlotRegistry == null)
            {
                Debug.LogWarning("LetterLauncher: BoxSlotRegistry is null.");
                return;
            }

            var targetBox = boxSlotRegistry.GetSlot(slotIndex);
            if (targetBox == null)
            {
                Debug.LogWarning($"LetterLauncher: No box at slot {slotIndex}.");
                return;
            }

            _isLaunching = true;

            _currentLetter.transform
                .DOMove(targetBox.transform.position, launchDuration)
                .SetEase(launchEase)
                .OnComplete(() => _isLaunching = false);
        }

        private void HandleLetterResolved(Letter.Letter letter, ServiceBox box, DeliveryResult result)
        {
            if (letter != _currentLetter) return;
            
            KillCurrentTween();
            _currentLetter = null;
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
