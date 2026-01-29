using System;
using Core;
using Scoring;
using UnityEngine;

namespace Gameplay.Difficulty
{
    /// <summary>
    /// Manages difficulty progression during gameplay.
    /// Listens to score changes and adjusts spawn rate, permutation timing, etc.
    /// </summary>
    public class DifficultyManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private GameSessionController session;
        [SerializeField] private DifficultySettings settings;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        private int _currentDifficultyLevel = 0;
        private float _currentSpawnDelay;
        private float _currentPermutationInterval;

        /// <summary>
        /// Fired when difficulty level increases.
        /// Provides new difficulty level and adjusted spawn delay.
        /// </summary>
        public event Action<int, float> OnDifficultyLevelChanged;

        /// <summary>
        /// Fired when it's time to permute boxes.
        /// </summary>
        public event Action OnBoxPermutationTriggered;

        public int CurrentDifficultyLevel => _currentDifficultyLevel;
        public float CurrentSpawnDelay => _currentSpawnDelay;

        private float _permutationTimer;
        private bool _permutationStarted;

        private void Start()
        {
            ValidateReferences();

            scoreSystem.OnScoringEvent += HandleScoringEvent;
            session.OnStateChanged += HandleStateChanged;

            Debug.Log("[DifficultyManager] Initialized and subscribed to events.");
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
                scoreSystem.OnScoringEvent -= HandleScoringEvent;

            if (session != null)
                session.OnStateChanged -= HandleStateChanged;

            Debug.Log("[DifficultyManager] Unsubscribed from events.");
        }

        private void Update()
        {
            if (session.CurrentState != GameSessionController.SessionState.Playing)
                return;

            // Handle box permutation timing
            if (settings.enableBoxPermutation)
            {
                UpdatePermutationTimer();
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState newState)
        {
            if (newState == GameSessionController.SessionState.Playing)
            {
                ResetDifficulty();

                if (settings.enableBoxPermutation && settings.shuffleAtStart)
                {
                    TriggerBoxPermutation();
                }
            }
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            // Calculate difficulty level from score
            int newLevel = settings.GetDifficultyLevel(evt.NewScore);

            // Check if level increased
            if (newLevel > _currentDifficultyLevel)
            {
                _currentDifficultyLevel = newLevel;
                UpdateDifficultyParameters();

                if (showDebugInfo)
                {
                    Debug.Log($"[DifficultyManager] Level increased to {_currentDifficultyLevel} " +
                              $"(spawn delay: {_currentSpawnDelay:F2}s, " +
                              $"permutation interval: {_currentPermutationInterval:F1}s)");
                }

                OnDifficultyLevelChanged?.Invoke(_currentDifficultyLevel, _currentSpawnDelay);
            }

            // Apply combo pressure if enabled
            if (settings.enableComboPressure && evt.NewCombo >= settings.comboPressureThreshold)
            {
                // This could modify spawn delay temporarily
                // For now, just log it
                if (showDebugInfo && evt.NewCombo == settings.comboPressureThreshold)
                {
                    Debug.Log($"[DifficultyManager] Combo pressure activated at x{evt.NewCombo}!");
                }
            }
        }

        private void UpdatePermutationTimer()
        {
            // Don't start permutation timer until initial delay has passed
            if (!_permutationStarted)
            {
                _permutationTimer += Time.deltaTime;
                if (_permutationTimer >= settings.initialPermutationDelay)
                {
                    _permutationStarted = true;
                    _permutationTimer = 0f;
                    TriggerBoxPermutation();
                }
                return;
            }

            // Normal permutation timing
            _permutationTimer += Time.deltaTime;
            if (_permutationTimer >= _currentPermutationInterval)
            {
                _permutationTimer = 0f;
                TriggerBoxPermutation();
            }
        }

        private void TriggerBoxPermutation()
        {
            if (showDebugInfo)
            {
                Debug.Log("[DifficultyManager] Triggering box permutation!");
            }

            OnBoxPermutationTriggered?.Invoke();
        }

        private void ResetDifficulty()
        {
            _currentDifficultyLevel = 0;
            _permutationTimer = 0f;
            _permutationStarted = false;
            UpdateDifficultyParameters();

            if (showDebugInfo)
            {
                Debug.Log($"[DifficultyManager] Reset to level 0 " +
                          $"(spawn delay: {_currentSpawnDelay:F2}s)");
            }
        }

        private void UpdateDifficultyParameters()
        {
            _currentSpawnDelay = settings.GetSpawnDelay(_currentDifficultyLevel);
            _currentPermutationInterval = settings.GetPermutationInterval(_currentDifficultyLevel);
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[DifficultyManager] ScoreSystem reference missing!");

            if (session == null)
                Debug.LogError("[DifficultyManager] GameSessionController reference missing!");

            if (settings == null)
                Debug.LogError("[DifficultyManager] DifficultySettings reference missing!");
        }
    }
}