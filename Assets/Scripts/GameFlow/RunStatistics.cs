using System;
using Core;
using Gameplay;
using Scoring;
using UnityEngine;

namespace GameFlow
{
    /// <summary>
    /// Tracks statistics for the current game run.
    /// Provides data for Game Over screen and potential high score tracking.
    /// </summary>
    public class RunStatistics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private HitResolver hitResolver;
        [SerializeField] private GameSessionController session;

        private int _totalLettersProcessed;
        private int _correctHits;
        private int _errorHits;
        private int _maxComboReached;
        private float _runStartTime;
        private float _runDuration;

        public int FinalScore => scoreSystem != null ? scoreSystem.Score : 0;
        public int TotalLettersProcessed => _totalLettersProcessed;
        public int CorrectHits => _correctHits;
        public int ErrorHits => _errorHits;
        public int MaxComboReached => _maxComboReached;
        public float RunDuration => _runDuration;
        public float Accuracy => _totalLettersProcessed > 0 ? (_correctHits / (float)_totalLettersProcessed) * 100f : 0f;

        private void Start()
        {
            ValidateReferences();

            hitResolver.OnLetterResolved += HandleLetterResolved;
            scoreSystem.OnScoringEvent += HandleScoringEvent;
            session.OnStateChanged += HandleStateChanged;

            Debug.Log("[RunStatistics] Initialized and subscribed to events.");
        }

        private void OnDestroy()
        {
            if (hitResolver != null)
                hitResolver.OnLetterResolved -= HandleLetterResolved;

            if (scoreSystem != null)
                scoreSystem.OnScoringEvent -= HandleScoringEvent;

            if (session != null)
                session.OnStateChanged -= HandleStateChanged;

            Debug.Log("[RunStatistics] Unsubscribed from events.");
        }

        private void HandleStateChanged(GameSessionController.SessionState newState)
        {
            switch (newState)
            {
                case GameSessionController.SessionState.Playing:
                    ResetStatistics();
                    _runStartTime = Time.time;
                    break;

                case GameSessionController.SessionState.GameOver:
                    _runDuration = Time.time - _runStartTime;
                    LogFinalStatistics();
                    break;
            }
        }

        private void HandleLetterResolved(LetterResolution resolution)
        {
            _totalLettersProcessed++;

            if (resolution.IsCorrect)
            {
                _correctHits++;
            }
            else
            {
                _errorHits++;
            }
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            if (evt.NewCombo > _maxComboReached)
            {
                _maxComboReached = evt.NewCombo;
            }
        }

        private void ResetStatistics()
        {
            _totalLettersProcessed = 0;
            _correctHits = 0;
            _errorHits = 0;
            _maxComboReached = 0;
            _runDuration = 0f;

            Debug.Log("[RunStatistics] Statistics reset for new run.");
        }

        private void LogFinalStatistics()
        {
            Debug.Log("=== RUN STATISTICS ===");
            Debug.Log($"Final Score: {FinalScore}");
            Debug.Log($"Total Letters: {TotalLettersProcessed}");
            Debug.Log($"Correct: {CorrectHits} | Errors: {ErrorHits}");
            Debug.Log($"Accuracy: {Accuracy:F1}%");
            Debug.Log($"Max Combo: x{MaxComboReached}");
            Debug.Log($"Run Duration: {RunDuration:F1}s");
            Debug.Log("======================");
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[RunStatistics] ScoreSystem reference missing!");

            if (hitResolver == null)
                Debug.LogError("[RunStatistics] HitResolver reference missing!");

            if (session == null)
                Debug.LogError("[RunStatistics] GameSessionController reference missing!");
        }
    }
}