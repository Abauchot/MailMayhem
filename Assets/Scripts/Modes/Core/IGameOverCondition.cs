using System;

namespace Modes.Core
{
    /// <summary>
    /// Interface for game over conditions.
    /// Each mode can implement its own condition (lives, timer, endless, etc.).
    /// </summary>
    public interface IGameOverCondition
    {
        /// <summary>
        /// Fired when game over condition is met.
        /// </summary>
        event Action OnGameOver;

        /// <summary>
        /// Initialize the condition (subscribe to events, etc.).
        /// </summary>
        void Initialize();

        /// <summary>
        /// Reset the condition to starting state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Check if game over condition is currently met.
        /// </summary>
        bool IsGameOver();

        /// <summary>
        /// Get current status as text (for UI).
        /// Example: "3 lives", "45s remaining", "Score: 5000"
        /// </summary>
        string GetStatusText();

        /// <summary>
        /// Get progress normalized (0-1) for UI bars.
        /// Example: 2/3 lives = 0.66, 30s/60s = 0.5
        /// </summary>
        float GetProgressNormalized();

        /// <summary>
        /// Cleanup when condition is no longer needed.
        /// </summary>
        void Cleanup();
    }
}