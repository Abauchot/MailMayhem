using System;
using Gameplay;
using Modes.Core;
using UnityEngine;

namespace Modes.Conditions
{
    /// <summary>
    /// Game over condition based on a countdown timer.
    /// Timer decreases over time. Correct hits add time, errors reduce time.
    /// 0 time = game over.
    /// </summary>
    public class TimeGameOverCondition : IGameOverCondition
    {
        public event Action OnGameOver;

        private readonly float _startTime;
        private readonly float _timePerCorrect;
        private readonly float _timePenaltyError;
        private readonly HitResolver _hitResolver;
        
        private float _currentTime;
        private bool _isInitialized;
        private bool _isPaused;

        public float CurrentTime => _currentTime;
        public float StartTime => _startTime;

        public TimeGameOverCondition(float startTime, float timePerCorrect, float timePenaltyError, HitResolver hitResolver)
        {
            this._startTime = startTime;
            this._timePerCorrect = timePerCorrect;
            this._timePenaltyError = timePenaltyError;
            this._hitResolver = hitResolver;
            this._currentTime = startTime;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            if (_hitResolver != null)
            {
                _hitResolver.OnLetterResolved += HandleLetterResolved;
            }

            _isInitialized = true;
            Debug.Log($"[TimeCondition] Initialized with {_startTime}s start time.");
        }

        public void Reset()
        {
            _currentTime = _startTime;
            _isPaused = false;
            Debug.Log($"[TimeCondition] Time reset to {_startTime}s.");
        }

        public bool IsGameOver()
        {
            return _currentTime <= 0f;
        }

        public string GetStatusText()
        {
            int minutes = Mathf.FloorToInt(_currentTime / 60f);
            int seconds = Mathf.FloorToInt(_currentTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        public float GetProgressNormalized()
        {
            return Mathf.Clamp01(_currentTime / _startTime);
        }

        public void Cleanup()
        {
            if (_hitResolver != null)
            {
                _hitResolver.OnLetterResolved -= HandleLetterResolved;
            }

            _isInitialized = false;
        }

        /// <summary>
        /// Update the timer (call this in Update loop).
        /// </summary>
        public void UpdateTimer(float deltaTime)
        {
            if (_isPaused || _currentTime <= 0f)
                return;

            _currentTime -= deltaTime;

            if (_currentTime <= 0f)
            {
                _currentTime = 0f;
                Debug.Log("[TimeCondition] ⏱️ Time's up - Game Over!");
                OnGameOver?.Invoke();
            }
        }

        /// <summary>
        /// Pause the timer.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resume the timer.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        private void HandleLetterResolved(LetterResolution resolution)
        {
            if (resolution.IsCorrect)
            {
                AddTime(_timePerCorrect);
            }
            else
            {
                RemoveTime(_timePenaltyError);
            }
        }

        private void AddTime(float amount)
        {
            _currentTime += amount;
            Debug.Log($"[TimeCondition] +{amount}s → {_currentTime:F1}s remaining");
        }

        private void RemoveTime(float amount)
        {
            _currentTime -= amount;
            _currentTime = Mathf.Max(0f, _currentTime);
            Debug.Log($"[TimeCondition] -{amount}s → {_currentTime:F1}s remaining");

            if (_currentTime <= 0f)
            {
                Debug.Log("[TimeCondition] ⏱️ Time's up - Game Over!");
                OnGameOver?.Invoke();
            }
        }
    }
}