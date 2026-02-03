using System;
using Core;
using Gameplay;
using Modes.Conditions;
using Modes.Core;
using UnityEngine;

namespace Modes.Implementations
{
    /// <summary>
    /// Classic arcade mode with 3 lives.
    /// Lose a life on each error. 0 lives = game over.
    /// NO direct singleton access - dependencies injected via Inspector.
    /// </summary>
    public class ClassicMode : GameMode
    {
        [Header("Classic Mode Settings")]
        [SerializeField] private int startingLives = 3;

        [Header("References")]
        [SerializeField] private HitResolver hitResolver;
        [SerializeField] private GameSessionController sessionController;

        private LivesGameOverCondition _livesCondition;

        private void Awake()
        {
            modeName = "Classic";
            modeDescription = "3 lives. Don't make mistakes!";
        }

        private void Start()
        {
            ValidateReferences();

            if (sessionController != null)
            {
                sessionController.OnStateChanged += HandleSessionStateChanged;
            }

            // Auto-initialize on start
            Initialize();
        }

        public override void Initialize()
        {
            // Create lives condition
            _livesCondition = new LivesGameOverCondition(startingLives, hitResolver);
            gameOverCondition = _livesCondition;

            base.Initialize();

            _livesCondition.Initialize();
        }

        public override void StartMode()
        {
            base.StartMode();

            _livesCondition?.Reset();
        }

        public override void EndMode()
        {
            base.EndMode();
        }

        protected override void HandleGameOver()
        {
            base.HandleGameOver();
            sessionController?.EndGame();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (sessionController != null)
            {
                sessionController.OnStateChanged -= HandleSessionStateChanged;
            }

            _livesCondition?.Cleanup();
        }

        private void HandleSessionStateChanged(GameSessionController.SessionState newState)
        {
            if (!enabled) return;

            switch (newState)
            {
                case GameSessionController.SessionState.Playing:
                    StartMode();
                    break;
                case GameSessionController.SessionState.GameOver:
                    EndMode();
                    break;
                case GameSessionController.SessionState.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public override string GetStatusText()
        {
            return _livesCondition == null ? "0 / 0" : $"❤️ {_livesCondition.GetStatusText()}";
        }

        public override float GetProgressNormalized()
        {
            return _livesCondition?.GetProgressNormalized() ?? 0f;
        }

        /// <summary>
        /// Public accessor for UI to get current lives.
        /// </summary>
        public int CurrentLives => _livesCondition?.CurrentLives ?? 0;

        /// <summary>
        /// Public accessor for UI to get max lives.
        /// </summary>
        public int MaxLives => _livesCondition?.MaxLives ?? 0;

        private void ValidateReferences()
        {
            if (hitResolver == null)
                Debug.LogError("[ClassicMode] HitResolver reference missing!");

            if (sessionController == null)
                Debug.LogError("[ClassicMode] SessionController reference missing! Drag GameSessionController GameObject into Inspector.");
        }
    }
}