using System;
using UnityEngine;

namespace Core
{
    public class GameSessionController : MonoBehaviour
    {
        public enum SessionState
        {
            Idle,
            Playing,
            GameOver
        }

        public static GameSessionController Instance { get; private set; }

        public SessionState CurrentState { get; private set; } = SessionState.Idle;

        public event Action OnGameStarted;
        public event Action OnGameEnded;
        public event Action OnGameRestarted;
        public event Action<SessionState> OnStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"[GameSessionController] Duplicate instance detected on '{gameObject.name}'. Destroying.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartGame()
        {
            if (CurrentState == SessionState.Playing)
                return;

            SetState(SessionState.Playing);
            OnGameStarted?.Invoke();
        }

        public void EndGame()
        {
            if (CurrentState != SessionState.Playing)
                return;

            SetState(SessionState.GameOver);
            OnGameEnded?.Invoke();
        }

        public void RestartGame()
        {
            SetState(SessionState.Idle);
            OnGameRestarted?.Invoke();
            StartGame();
        }

        private void SetState(SessionState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
