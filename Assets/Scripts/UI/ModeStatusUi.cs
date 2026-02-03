using Modes.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Generic UI that displays status for the active game mode.
    /// Adapts display based on mode type (lives, timer, etc.).
    /// </summary>
    public class ModeStatusUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameMode activeMode;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Image progressBar;
        [SerializeField] private GameObject statusPanel;

        [Header("Settings")]
        [SerializeField] private bool hideWhenNoMode = true;

        private void Start()
        {
            ValidateReferences();
            UpdateVisibility();
        }

        private void Update()
        {
            if (!activeMode || !activeMode.IsActive)
            {
                UpdateVisibility();
                return;
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Update text
            if (statusText)
            {
                statusText.text = activeMode.GetStatusText();
            }

            // Update progress bar
            if (!progressBar) return;
            progressBar.fillAmount = activeMode.GetProgressNormalized();

            // Color based on progress (optional)
            Color barColor = GetProgressColor(activeMode.GetProgressNormalized());
            progressBar.color = barColor;
        }

        private Color GetProgressColor(float progress)
        {
            // Green → Yellow → Red based on progress
            return progress > 0.5f ? Color.Lerp(Color.yellow, Color.green, 
                    (progress - 0.5f) * 2f)
                     : Color.Lerp(Color.red, Color.yellow, progress * 2f);
        }

        private void UpdateVisibility()
        {
            if (!statusPanel)
                return;

            bool shouldShow = activeMode && activeMode.IsActive;

            if (hideWhenNoMode)
            {
                statusPanel.SetActive(shouldShow);
            }
        }

        private void ValidateReferences()
        {
            if (activeMode == null)
                Debug.LogWarning("[ModeStatusUI] No active mode assigned!");

            if (statusText == null)
                Debug.LogWarning("[ModeStatusUI] Status text missing!");
        }

        /// <summary>
        /// Public method to change the active mode at runtime.
        /// </summary>
        public void SetActiveMode(GameMode mode)
        {
            activeMode = mode;
            UpdateVisibility();
        }
    }
}