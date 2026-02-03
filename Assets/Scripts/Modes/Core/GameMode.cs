using System;
using UnityEngine;

namespace Modes.Core
{
    /// <summary>
    /// Base class for all game modes in Mail Mayhem.
    /// Each mode defines its own game over conditions, difficulty settings, and UI.
    /// </summary>
    public abstract class GameMode : MonoBehaviour
    {
        [Header("Mode Info")]
        [SerializeField] protected string modeName;
        [SerializeField] protected string modeDescription;

        protected IGameOverCondition gameOverCondition;
        protected bool isActive;

        /// <summary>
        /// Event fired when this mode should trigger game over.
        /// </summary>
        public event Action OnGameOver;

        public string ModeName => modeName;
        public string ModeDescription => modeDescription;
        public bool IsActive => isActive;

        /// <summary>
        /// Initialize the mode (called when selected).
        /// </summary>
        public virtual void Initialize()
        {
            if (gameOverCondition != null)
            {
                gameOverCondition.OnGameOver += HandleGameOver;
            }

            Debug.Log($"[GameMode] {modeName} initialized.");
        }

        /// <summary>
        /// Start the mode (called when gameplay begins).
        /// </summary>
        public virtual void StartMode()
        {
            isActive = true;

            gameOverCondition?.Reset();

            Debug.Log($"[GameMode] {modeName} started.");
        }

        /// <summary>
        /// End the mode (called when gameplay ends).
        /// </summary>
        public virtual void EndMode()
        {
            isActive = false;

            Debug.Log($"[GameMode] {modeName} ended.");
        }

        /// <summary>
        /// Cleanup when mode is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (gameOverCondition != null)
            {
                gameOverCondition.OnGameOver -= HandleGameOver;
            }
        }

        protected virtual void HandleGameOver()
        {
            Debug.Log($"[GameMode] {modeName} - Game Over triggered!");
            OnGameOver?.Invoke();
        }

        /// <summary>
        /// Get the current game over condition status (for UI display).
        /// </summary>
        public abstract string GetStatusText();

        /// <summary>
        /// Get progress (0-1) for UI display (e.g., lives remaining, time remaining).
        /// </summary>
        public abstract float GetProgressNormalized();
    }
}