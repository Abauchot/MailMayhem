using Core;
using Gameplay;
using Modes.Conditions;
using Modes.Core;
using UnityEngine;

namespace Modes.Implementations
{
    /// <summary>
    /// Time Attack mode - race against the clock!
    /// Timer counts down. Correct hits add time, errors subtract time.
    /// 0 time = game over.
    /// NO direct singleton access - dependencies injected via Inspector.
    /// </summary>
    public class TimeAttackMode : GameMode
    {
        [Header("Time Attack Settings")]
        [Tooltip("Starting time in seconds")]
        [SerializeField] private float startTime = 60f;

        [Tooltip("Time bonus for correct hits")]
        [SerializeField] private float timePerCorrect = 2f;

        [Tooltip("Time penalty for errors")]
        [SerializeField] private float timePenaltyError = 5f;

        [Header("References")]
        [SerializeField] private HitResolver hitResolver;
        [SerializeField] private GameSessionController sessionController;

        private TimeGameOverCondition _timeCondition;

        private void Awake()
        {
            modeName = "Time Attack";
            modeDescription = "Race against the clock!";
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
            // Create time condition
            _timeCondition = new TimeGameOverCondition(startTime, timePerCorrect, timePenaltyError, hitResolver);
            gameOverCondition = _timeCondition;

            base.Initialize();

            _timeCondition.Initialize();
        }

        public override void StartMode()
        {
            base.StartMode();

            if (_timeCondition == null) return;
            _timeCondition.Reset();
            _timeCondition.Resume();
        }

        public override void EndMode()
        {
            base.EndMode();

            if (_timeCondition != null)
            {
                _timeCondition.Pause();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (sessionController != null)
            {
                sessionController.OnStateChanged -= HandleSessionStateChanged;
            }

            _timeCondition?.Cleanup();
        }

        protected override void HandleGameOver()
        {
            base.HandleGameOver();
            sessionController?.EndGame();
        }

        private void Update()
        {
            // Update timer if mode is active
            if (isActive && _timeCondition != null)
            {
                _timeCondition.UpdateTimer(Time.deltaTime);
            }
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
                default:
                {
                    if (newState == GameSessionController.SessionState.Paused)
                    {
                        _timeCondition?.Pause();
                    }

                    break;
                }
            }
        }

        public override string GetStatusText()
        {
            return _timeCondition == null ? "00:00" : $"⏱️ {_timeCondition.GetStatusText()}";
        }

        public override float GetProgressNormalized()
        {
            return _timeCondition?.GetProgressNormalized() ?? 0f;
        }

        /// <summary>
        /// Public accessor for UI to get current time.
        /// </summary>
        public float CurrentTime => _timeCondition?.CurrentTime ?? 0f;

        /// <summary>
        /// Public accessor for UI to get start time.
        /// </summary>
        public float StartTime => _timeCondition?.StartTime ?? 0f;

        private void ValidateReferences()
        {
            if (hitResolver == null)
                Debug.LogError("[TimeAttackMode] HitResolver reference missing!");

            if (sessionController == null)
                Debug.LogError("[TimeAttackMode] SessionController reference missing! Drag GameSessionController GameObject into Inspector.");
        }
    }
}