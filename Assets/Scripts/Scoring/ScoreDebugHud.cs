using TMPro;
using UnityEngine;

namespace Scoring
{
    public class ScoreDebugHud : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;

        private void Start()
        {
            if (scoreSystem == null)
            {
                Debug.LogError("[ScoreDebugHud] ScoreSystem reference is missing.");
                enabled = false;
                return;
            }

            if (scoreText == null)
            {
                Debug.LogError("[ScoreDebugHud] ScoreText reference is missing.");
            }

            if (comboText == null)
            {
                Debug.LogError("[ScoreDebugHud] ComboText reference is missing.");
            }

            scoreSystem.OnScoreChanged += HandleScoreChanged;
            scoreSystem.OnComboChanged += HandleComboChanged;

            RefreshUI();

            Debug.Log("[ScoreDebugHud] Initialized and subscribed to ScoreSystem events.");
        }

        private void OnDisable()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoreChanged -= HandleScoreChanged;
                scoreSystem.OnComboChanged -= HandleComboChanged;
            }
        }

        private void HandleScoreChanged(int newScore)
        {
            UpdateScoreText(newScore);
        }

        private void HandleComboChanged(int newCombo)
        {
            UpdateComboText(newCombo);
        }

        private void RefreshUI()
        {
            UpdateScoreText(scoreSystem.Score);
            UpdateComboText(scoreSystem.Combo);
        }

        private void UpdateScoreText(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {score}";
            }
        }

        private void UpdateComboText(int combo)
        {
            if (comboText != null)
            {
                comboText.text = $"COMBO: x{combo}";
            }
        }
    }
}
