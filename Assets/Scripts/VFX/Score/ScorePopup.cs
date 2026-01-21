using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VFX.Score
{
    /// <summary>
    /// Individual score popup that animates upward and fades out.
    /// Automatically destroys itself after animation completes.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class ScorePopup : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float floatDistance = 1.5f;
        [SerializeField] private float duration = 0.8f;
        [SerializeField] private Ease floatEase = Ease.OutQuad;
        [SerializeField] private Ease fadeEase = Ease.InQuad;

        private TextMeshPro _textMesh;
        private Sequence _animationSequence;

        private void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();
        }

        /// <summary>
        /// Initialize and play the popup animation.
        /// </summary>
        /// <param name="points">Points to display (e.g., 100, 200, 300)</param>
        /// <param name="comboMultiplier">Current combo for visual scaling/color</param>
        public void Play(int points, int comboMultiplier)
        {
            if (_textMesh == null)
            {
                Debug.LogError("[ScorePopup] TextMeshPro component missing!");
                Destroy(gameObject);
                return;
            }
            
            _textMesh.text = $"+{points}";
            
            ApplyComboStyling(comboMultiplier);
            
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + Vector3.up * floatDistance;

            _animationSequence = DOTween.Sequence();
            _animationSequence.Append(
                transform.DOMove(endPosition, duration).SetEase(floatEase)
            );
            
            _animationSequence.Join(
                _textMesh.DOFade(0f, duration).SetEase(fadeEase)
            );
            
            _animationSequence.OnComplete(() => Destroy(gameObject));
        }

        /// <summary>
        /// Apply visual styling based on combo level.
        /// Higher combos = bigger text, more vibrant colors.
        /// </summary>
        private void ApplyComboStyling(int combo)
        {
            float fontSize = 3f;
            
            Color color = new Color(1f, 0.95f, 0.4f); // Warm yellow
            
            switch (combo)
            {
                case >= 5:
                    fontSize = 4.5f;
                    color = new Color(1f, 0.5f, 0f); // Orange
                    break;
                case >= 3:
                    fontSize = 3.8f;
                    color = new Color(1f, 0.8f, 0.2f); // Gold
                    break;
                case >= 2:
                    fontSize = 3.3f;
                    break;
            }

            _textMesh.fontSize = fontSize;
            _textMesh.color = color;
        }

        private void OnDestroy()
        {
            if (_animationSequence != null && _animationSequence.IsActive())
            {
                _animationSequence.Kill();
            }
        }
    }
}