using System;
using Gameplay;
using Modes.Core;
using UnityEngine;

namespace Modes.Conditions
{
    /// <summary>
    /// Game over condition based on lives.
    /// Player loses a life on each error. 0 lives = game over.
    /// </summary>
    public class LivesGameOverCondition : IGameOverCondition
    {
        public event Action OnGameOver;

        private readonly int _maxLives;
        private readonly HitResolver _hitResolver;
        private int _currentLives;
        private bool _isInitialized;

        public int CurrentLives => _currentLives;
        public int MaxLives => _maxLives;

        public LivesGameOverCondition(int maxLives, HitResolver hitResolver)
        {
            this._maxLives = maxLives;
            this._hitResolver = hitResolver;
            this._currentLives = maxLives;
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
            Debug.Log($"[LivesCondition] Initialized with {_maxLives} lives.");
        }

        public void Reset()
        {
            _currentLives = _maxLives;
            Debug.Log($"[LivesCondition] Lives reset to {_maxLives}.");
        }

        public bool IsGameOver()
        {
            return _currentLives <= 0;
        }

        public string GetStatusText()
        {
            return $"{_currentLives} / {_maxLives}";
        }

        public float GetProgressNormalized()
        {
            return (float)_currentLives / _maxLives;
        }

        public void Cleanup()
        {
            if (_hitResolver != null)
            {
                _hitResolver.OnLetterResolved -= HandleLetterResolved;
            }

            _isInitialized = false;
        }

        private void HandleLetterResolved(LetterResolution resolution)
        {
            if (!resolution.IsCorrect)
            {
                LoseLife();
            }
        }

        private void LoseLife()
        {
            if (_currentLives <= 0)
                return;

            _currentLives--;
            Debug.Log($"[LivesCondition] Life lost! Remaining: {_currentLives}/{_maxLives}");

            if (_currentLives <= 0)
            {
                Debug.Log("[LivesCondition] ❌ No lives remaining - Game Over!");
                OnGameOver?.Invoke();
            }
        }
    }
}