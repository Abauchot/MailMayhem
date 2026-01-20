using Scoring;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DebugScoreHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;

        private void Start()
        {
            ValidateReferences();
            
            scoreSystem.OnScoringEvent += UpdateDisplay;
            
            // Initialize display
            UpdateDisplay(new ScoringEvent(
                isCorrect: true,
                pointsDelta: 0,
                newScore: 0,
                newCombo: 0,
                letter: null,
                frame: 0
            ));
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
                scoreSystem.OnScoringEvent -= UpdateDisplay;
        }

        private void UpdateDisplay(ScoringEvent evt)
        {
            scoreText.text = $"SCORE: {evt.NewScore}";
            comboText.text = $"COMBO: x{evt.NewCombo}";
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[DebugScoreHUD] ScoreSystem reference missing!");
            
            if (scoreText == null)
                Debug.LogError("[DebugScoreHUD] ScoreText reference missing!");
            
            if (comboText == null)
                Debug.LogError("[DebugScoreHUD] ComboText reference missing!");
        }
    }
}