using Scoring;
using UnityEngine;

namespace VFX.Boxes
{
    /// <summary>
    /// Coordinates box visual feedback based on scoring events.
    /// Listens to ScoreSystem and triggers appropriate BoxFeedback on hit boxes.
    /// Centralized controller - only one instance needed in the scene.
    /// </summary>
    public class BoxFeedbackCoordinator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;

        private void Start()
        {
            ValidateReferences();
            scoreSystem.OnScoringEvent += HandleScoringEvent;
            Debug.Log("[BoxFeedbackCoordinator] Initialized and subscribed to scoring events.");
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoringEvent -= HandleScoringEvent;
            }
            Debug.Log("[BoxFeedbackCoordinator] Unsubscribed from scoring events.");
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            if (evt.HitBox == null)
            {
                Debug.LogWarning("[BoxFeedbackCoordinator] ScoringEvent has null HitBox reference.");
                return;
            }
            
            BoxFeedback feedback = evt.HitBox.GetComponent<BoxFeedback>();
            if (feedback == null)
            {
                Debug.LogWarning($"[BoxFeedbackCoordinator] BoxFeedback component missing on {evt.HitBox.gameObject.name}");
                return;
            }
            
            if (evt.IsCorrect)
            {
                feedback.PlayCorrectFeedback();
            }
            else
            {
                feedback.PlayIncorrectFeedback();
            }
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[BoxFeedbackCoordinator] ScoreSystem reference missing!");
        }
    }
}