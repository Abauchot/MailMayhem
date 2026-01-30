using DG.Tweening;
using Gameplay.Boxes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Displays visual warnings when box permutations are about to occur.
    /// Shows the mode type and countdown.
    /// </summary>
    public class PermutationWarningUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoxPermutationController permutationController;

        [Header("UI Elements")]
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TextMeshProUGUI modeText;
        [SerializeField] private TextMeshProUGUI countdownText;

        [Header("Mode Colors")]
        [SerializeField] private Color mirrorColor = new Color(0.2f, 0.8f, 1f); // Cyan
        [SerializeField] private Color swapColor = new Color(1f, 0.8f, 0.2f);   // Orange
        [SerializeField] private Color shuffleColor = new Color(1f, 0.3f, 0.3f); // Red

        [Header("Animation")]
        [SerializeField] private float pulseScale = 1.1f;
        [SerializeField] private float pulseDuration = 0.5f;

        [Header("Mode Labels (French)")]
        [SerializeField] private string mirrorLabel = "MIRROR";
        [SerializeField] private string swapLabel = "SWAP";
        [SerializeField] private string shuffleLabel = "SHUFFLE";

        private Sequence _pulseSequence;
        private float _warningTimeRemaining;
        private bool _warningActive;

        private void Start()
        {
            ValidateReferences();

            if (permutationController != null)
            {
                permutationController.OnPermutationWarning += HandleWarning;
                permutationController.OnPermutationComplete += HandleComplete;
            }

            if (warningPanel != null)
                warningPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (permutationController != null)
            {
                permutationController.OnPermutationWarning -= HandleWarning;
                permutationController.OnPermutationComplete -= HandleComplete;
            }

            _pulseSequence?.Kill();
        }

        private void Update()
        {
            if (_warningActive)
            {
                _warningTimeRemaining -= Time.deltaTime;

                if (countdownText != null)
                {
                    countdownText.text = Mathf.Max(0, _warningTimeRemaining).ToString("F1") + "s";
                }

                if (_warningTimeRemaining <= 0)
                {
                    _warningActive = false;
                }
            }
        }

        private void HandleWarning(BoxPermutationController.PermutationMode mode, float duration)
        {
            _warningTimeRemaining = duration;
            _warningActive = true;

            if (warningPanel != null)
                warningPanel.SetActive(true);

            // Set mode text and color
            string modeLabel = GetModeLabel(mode);
            Color modeColor = GetModeColor(mode);

            if (modeText != null)
            {
                modeText.text = modeLabel;
                modeText.color = modeColor;
            }
            

            // Start pulse animation
            StartPulseAnimation();

            Debug.Log($"[WarningUI] Showing warning: {mode} for {duration}s");
        }

        private void HandleComplete()
        {
            _warningActive = false;

            if (warningPanel != null)
                warningPanel.SetActive(false);

            _pulseSequence?.Kill();
        }

        private string GetModeLabel(BoxPermutationController.PermutationMode mode)
        {
            switch (mode)
            {
                case BoxPermutationController.PermutationMode.Mirror:
                    return mirrorLabel;
                case BoxPermutationController.PermutationMode.Swap:
                    return swapLabel;
                case BoxPermutationController.PermutationMode.Shuffle:
                    return shuffleLabel;
                default:
                    return "???";
            }
        }

        private Color GetModeColor(BoxPermutationController.PermutationMode mode)
        {
            switch (mode)
            {
                case BoxPermutationController.PermutationMode.Mirror:
                    return mirrorColor;
                case BoxPermutationController.PermutationMode.Swap:
                    return swapColor;
                case BoxPermutationController.PermutationMode.Shuffle:
                    return shuffleColor;
                default:
                    return Color.white;
            }
        }

        private void StartPulseAnimation()
        {
            _pulseSequence?.Kill();

            if (warningPanel == null) return;

            _pulseSequence = DOTween.Sequence();
            _pulseSequence.Append(warningPanel.transform.DOScale(pulseScale, pulseDuration).SetEase(Ease.InOutSine));
            _pulseSequence.Append(warningPanel.transform.DOScale(1f, pulseDuration).SetEase(Ease.InOutSine));
            _pulseSequence.SetLoops(-1); // Infinite loop
        }

        private void ValidateReferences()
        {
            if (permutationController == null)
                Debug.LogWarning("[WarningUI] PermutationController reference missing!");

            if (warningPanel == null)
                Debug.LogWarning("[WarningUI] Warning panel missing!");

            if (modeText == null)
                Debug.LogWarning("[WarningUI] Mode text missing!");
        }
    }
}