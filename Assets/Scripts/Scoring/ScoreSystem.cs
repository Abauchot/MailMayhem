using System;
using Core;
using Gameplay;
using UnityEngine;

namespace Scoring
{
    public readonly struct ScoringEvent
    {
        public bool IsCorrect { get; }
        public int PointsDelta { get; }
        public int NewScore { get; }
        public int NewCombo { get; }
        public Gameplay.Letter.Letter Letter { get; }
        public int Frame { get; }

        public ScoringEvent(
            bool isCorrect, 
            int pointsDelta,
            int newScore, 
            int newCombo,
            Gameplay.Letter.Letter letter,
            int frame
            )
        {
            IsCorrect = isCorrect;
            PointsDelta = pointsDelta;
            NewScore = newScore;
            NewCombo = newCombo;
            Letter = letter;
            Frame = frame;
        }
    }

    /// <summary>
    /// Manages score and combo. Listens for letter resolutions and session state,
    /// calculates points (basePoints * combo), raises scoring events, and resets
    /// score/combo when the game state changes.
    /// </summary>
    public class ScoreSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HitResolver hitResolver;
        [SerializeField] private GameSessionController session;

        [Header("Settings")]
        [SerializeField] private int basePoints = 100;
        [SerializeField] private bool enableDoubleResolveGuard = true;

        private int _score;
        private int _combo;

        private Gameplay.Letter.Letter _lastResolvedLetter;
        private int _lastResolveFrame;
        
        public int Score => _score;
        
        public int Combo => _combo;

        /// <summary>
        /// Fired on each scoring update. Provides IsCorrect, PointsDelta, NewScore,
        /// NewCombo, Letter and Frame for UI, audio and VFX consumers.
        /// </summary>
        public event Action<ScoringEvent> OnScoringEvent;

        private void Awake()
        {
            _score = 0;
            _combo = 0;
            _lastResolvedLetter = null;
            _lastResolveFrame = -1;
        }

        private void Start()
        {
            if (hitResolver == null)
            {
                Debug.LogError("[ScoreSystem] HitResolver reference is missing.");
                return;
            }

            if (session == null)
            {
                Debug.LogError("[ScoreSystem] GameSessionController reference is missing.");
                return;
            }

            hitResolver.OnLetterResolved += HandleLetterResolved;
            session.OnStateChanged += HandleStateChanged;

            Debug.Log("[ScoreSystem] Initialized and subscribed to events.");
        }

        private void OnDisable()
        {
            if (hitResolver != null)
                hitResolver.OnLetterResolved -= HandleLetterResolved;

            if (session != null)
                session.OnStateChanged -= HandleStateChanged;

            Debug.Log("[ScoreSystem] Unsubscribed from events.");
        }

        private void HandleLetterResolved(LetterResolution resolution)
        {
            if (enableDoubleResolveGuard && IsDuplicateResolve(resolution.Letter))
            {
                Debug.LogWarning($"[ScoreSystem] Ignoring duplicate resolve for letter: {resolution.Letter.Symbol}");
                return;
            }

            _lastResolvedLetter = resolution.Letter;
            _lastResolveFrame = Time.frameCount;

            int pointsDelta = 0;

            if (resolution.IsCorrect)
            {
                _combo++;
                pointsDelta = basePoints * Math.Max(1, _combo);
                _score += pointsDelta;

                Debug.Log($"[ScoreSystem] correct -> combo={_combo} score={_score}");
            }
            else
            {
                _combo = 0;

                Debug.Log("[ScoreSystem] error -> combo reset");
            }

            var scoringEvent = new ScoringEvent(
                isCorrect: resolution.IsCorrect,
                pointsDelta: pointsDelta,
                newScore: _score,
                newCombo: _combo,
                letter: resolution.Letter,
                frame: Time.frameCount
            );
            OnScoringEvent?.Invoke(scoringEvent);
        }

        private void HandleStateChanged(GameSessionController.SessionState newState)
        {
            switch (newState)
            {
                case GameSessionController.SessionState.Idle:
                case GameSessionController.SessionState.Playing:
                    ResetAll(newState);
                    break;

                case GameSessionController.SessionState.GameOver:
                    ResetComboAndTracking(newState);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void ResetAll(GameSessionController.SessionState state)
        {
            _score = 0;
            _combo = 0;
            ClearResolveTracking();

            Debug.Log($"[ScoreSystem] Reset: score=0 combo=0 (state={state})");
        }

        private void ResetComboAndTracking(GameSessionController.SessionState state)
        {
            _combo = 0;
            ClearResolveTracking();

            Debug.Log($"[ScoreSystem] Combo reset: combo=0 (state={state})");
        }

        private bool IsDuplicateResolve(Gameplay.Letter.Letter letter)
        {
            if (letter == null)
                return false;

            if (letter == _lastResolvedLetter)
                return true;
            
            return Time.frameCount == _lastResolveFrame && _lastResolvedLetter != null;
        }

        private void ClearResolveTracking()
        {
            _lastResolvedLetter = null;
            _lastResolveFrame = -1;
        }

#if UNITY_EDITOR
        public void ForceResetForDebug()
        {
            ResetAll(GameSessionController.SessionState.Playing);
            Debug.Log("[ScoreSystem] ForceResetForDebug called.");
        }
#endif
    }
}