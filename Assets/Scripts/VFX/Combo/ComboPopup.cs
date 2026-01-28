using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VFX.Combo
{
    /// <summary>
    /// Special popup for combo milestones (e.g., "x5 COMBO!").
    /// Appears at a fixed screen position with dramatic scaling animation.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class ComboPopup : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private float scaleMultiplier = 1.3f;
        [SerializeField] private Ease scaleEase = Ease.OutBack;
        [SerializeField] private Ease fadeEase = Ease.InQuad;

        private TextMeshPro _textMesh;
        private Sequence _animationSequence;

        private void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();
        }

        /// <summary>
        /// Play the combo celebration animation.
        /// </summary>
        /// <param name="comboCount">The combo milestone being celebrated</param>
        public void Play(int comboCount)
        {
            if (_textMesh == null)
            {
                Debug.LogError("[ComboPopup] TextMeshPro component missing!");
                Destroy(gameObject);
                return;
            }


            _textMesh.text = GetComboText(comboCount);
            
            ApplyComboStyling(comboCount);
            
            transform.localScale = Vector3.zero;

            _animationSequence = DOTween.Sequence();
            
            _animationSequence.Append(
                transform.DOScale(Vector3.one * scaleMultiplier, duration * 0.4f)
                    .SetEase(scaleEase)
            );
            
            _animationSequence.AppendInterval(duration * 0.2f);
            
            _animationSequence.Append(
                _textMesh.DOFade(0f, duration * 0.4f).SetEase(fadeEase)
            );
            
            _animationSequence.OnComplete(() => Destroy(gameObject));

            Debug.Log($"[ComboPopup] Playing combo celebration: {_textMesh.text}");
        }

        private string GetComboText(int combo)
        {
            return combo switch
            {
                >= 20 => $"×{combo} UNSTOPPABLE!",
                >= 15 => $"×{combo} AMAZING!",
                >= 10 => $"×{combo} INCREDIBLE!",
                >= 5 or >= 3 => $"×{combo} COMBO!",
                _ => $"×{combo}"
            };
        }

        private void ApplyComboStyling(int combo)
        {

            float fontSize = 5f;
            Color color = new Color(1f, 0.85f, 0.2f); // Gold

            switch (combo)
            {
                // Escalate with combo level
                case >= 20:
                    fontSize = 7f;
                    color = new Color(1f, 0.2f, 0.8f); // Pink/Magenta
                    break;
                case >= 15:
                    fontSize = 6.5f;
                    color = new Color(0.5f, 0.3f, 1f); // Purple
                    break;
                case >= 10:
                    fontSize = 6f;
                    color = new Color(1f, 0.4f, 0.1f); // Orange-Red
                    break;
                case >= 5:
                    fontSize = 5.5f;
                    color = new Color(1f, 0.6f, 0.1f); // Orange
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