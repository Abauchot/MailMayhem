using Core;
using GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the Game Over UI.
    /// Displays run statistics and provides restart/menu options.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSessionController session;
        [SerializeField] private RunStatistics runStats;

        [Header("UI Elements")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI maxComboText;
        [SerializeField] private TextMeshProUGUI totalLettersText;
        [SerializeField] private TextMeshProUGUI durationText;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            ValidateReferences();

            if (session != null)
            {
                session.OnStateChanged += HandleStateChanged;
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            // Initially hide game over screen
            HideGameOver();

            Debug.Log("[GameOverUI] Initialized.");
        }

        private void OnDestroy()
        {
            if (session != null)
            {
                session.OnStateChanged -= HandleStateChanged;
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState newState)
        {
            switch (newState)
            {
                case GameSessionController.SessionState.GameOver:
                    ShowGameOver();
                    break;

                case GameSessionController.SessionState.Idle:
                case GameSessionController.SessionState.Playing:
                    HideGameOver();
                    break;
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            UpdateStatisticsDisplay();

            Debug.Log("[GameOverUI] Game Over screen shown.");
        }

        private void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            Debug.Log("[GameOverUI] Game Over screen hidden.");
        }

        private void UpdateStatisticsDisplay()
        {
            if (runStats == null)
            {
                Debug.LogWarning("[GameOverUI] RunStatistics reference missing, can't display stats!");
                return;
            }

            if (finalScoreText != null)
                finalScoreText.text = $"FINAL SCORE: {runStats.FinalScore}";

            if (accuracyText != null)
                accuracyText.text = $"ACCURACY: {runStats.Accuracy:F1}%";

            if (maxComboText != null)
                maxComboText.text = $"MAX COMBO: ×{runStats.MaxComboReached}";

            if (totalLettersText != null)
                totalLettersText.text = $"LETTERS SORTED: {runStats.TotalLettersProcessed}";

            if (durationText != null)
            {
                int minutes = Mathf.FloorToInt(runStats.RunDuration / 60f);
                int seconds = Mathf.FloorToInt(runStats.RunDuration % 60f);
                durationText.text = $"TIME: {minutes:00}:{seconds:00}";
            }
        }

        private void OnRestartClicked()
        {
            Debug.Log("[GameOverUI] Restart button clicked.");

            if (session != null)
            {
                session.StartGame();
            }
        }

        private void OnMainMenuClicked()
        {
            Debug.Log("[GameOverUI] Main Menu button clicked.");

            if (session != null)
            {
                session.ReturnToIdle();
            }
        }

        private void ValidateReferences()
        {
            if (session == null)
                Debug.LogError("[GameOverUI] GameSessionController reference missing!");

            if (runStats == null)
                Debug.LogError("[GameOverUI] RunStatistics reference missing!");

            if (gameOverPanel == null)
                Debug.LogWarning("[GameOverUI] GameOverPanel not assigned!");

            if (restartButton == null)
                Debug.LogWarning("[GameOverUI] RestartButton not assigned!");

            if (mainMenuButton == null)
                Debug.LogWarning("[GameOverUI] MainMenuButton not assigned!");
        }
    }
}