using DG.Tweening;
using UnityEngine;

namespace VFX.Boxes
{
    /// <summary>
    /// Provides visual feedback when a ServiceBox is hit by a letter.
    /// Handles flash and shake effects based on hit correctness.
    /// Attach to each ServiceBox GameObject.
    /// </summary>
    public class BoxFeedback : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Flash Settings")]
        [SerializeField] private Color correctFlashColor = Color.green;
        [SerializeField] private Color incorrectFlashColor = Color.red;
        [SerializeField] private float flashDuration = 0.15f;

        [Header("Shake Settings")]
        [SerializeField] private float shakeStrength = 0.2f;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private int shakeVibrato = 10;

        private Color _originalColor;
        private Vector3 _shakeBasePosition;
        private Sequence _currentSequence;

        private void Awake()
        {
            if (spriteRenderer != null)
            {
                _originalColor = spriteRenderer.color;
            }
        }

        private void Start()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError($"[BoxFeedback] SpriteRenderer missing on {gameObject.name}");
            }
        }
        public void PlayCorrectFeedback()
        {
            KillCurrentAnimation();
            
            _shakeBasePosition = transform.localPosition;
            
            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(
                spriteRenderer.DOColor(correctFlashColor, flashDuration * 0.5f)
            );
            _currentSequence.Append(
                spriteRenderer.DOColor(_originalColor, flashDuration * 0.5f)
            );
            
            _currentSequence.Join(
                transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato)
                    .OnComplete(() => transform.localPosition = _shakeBasePosition)
            );

            Debug.Log($"[BoxFeedback] Playing correct feedback on {gameObject.name}");
        }

        public void PlayIncorrectFeedback()
        {
            KillCurrentAnimation();
            
            _shakeBasePosition = transform.localPosition;

            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(
                spriteRenderer.DOColor(incorrectFlashColor, flashDuration * 0.5f)
            );
            _currentSequence.Append(
                spriteRenderer.DOColor(_originalColor, flashDuration * 0.5f)
            );
            
            _currentSequence.Join(
                transform.DOShakePosition(shakeDuration, shakeStrength * 1.5f, shakeVibrato)
                    .OnComplete(() => transform.localPosition = _shakeBasePosition)
            );

            Debug.Log($"[BoxFeedback] Playing incorrect feedback on {gameObject.name}");
        }

        private void KillCurrentAnimation()
        {
            bool wasActive = _currentSequence != null && _currentSequence.IsActive();
            
            if (wasActive)
            {
                _currentSequence.Kill();
                transform.localPosition = _shakeBasePosition;
            }
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = _originalColor;
            }
        }

        private void OnDestroy()
        {
            KillCurrentAnimation();
        }
    }
}