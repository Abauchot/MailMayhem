using System;
using Core;
using Modes.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Controls the Main Menu UI.
    /// Lets the player pick a game mode, then loads the Gameplay scene.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Mode Buttons")]
        [SerializeField] private Button classicModeButton;
        [SerializeField] private Button timeAttackModeButton;

        [Header("UI Elements")]
        [SerializeField] private GameObject menuPanel;

        private GameSessionController _session;

        private void Start()
        {
            _session = GameSessionController.Instance;

            ValidateReferences();

            if (classicModeButton != null)
                classicModeButton.onClick.AddListener(OnClassicModeClicked);

            if (timeAttackModeButton != null)
                timeAttackModeButton.onClick.AddListener(OnTimeAttackModeClicked);

            if (_session != null)
            {
                _session.OnStateChanged += HandleStateChanged;
            }

            if (_session != null && _session.CurrentState == GameSessionController.SessionState.Idle)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }

            Debug.Log("[MainMenuUI] Initialized.");
        }

        private void OnDestroy()
        {
            if (classicModeButton != null)
                classicModeButton.onClick.RemoveListener(OnClassicModeClicked);

            if (timeAttackModeButton != null)
                timeAttackModeButton.onClick.RemoveListener(OnTimeAttackModeClicked);

            if (_session != null)
            {
                _session.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState newState)
        {
            switch (newState)
            {
                case GameSessionController.SessionState.Idle:
                    ShowMenu();
                    break;

                case GameSessionController.SessionState.Playing:
                case GameSessionController.SessionState.GameOver:
                    HideMenu();
                    break;
                case GameSessionController.SessionState.Paused:
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void OnClassicModeClicked()
        {
            StartGameWithMode(GameModeType.Classic);
        }

        private void OnTimeAttackModeClicked()
        {
            StartGameWithMode(GameModeType.TimeAttack);
        }

        private void StartGameWithMode(GameModeType mode)
        {
            Debug.Log($"[MainMenuUI] Starting game in {mode} mode.");

            GameModeSelection.SelectedMode = mode;
            SceneManager.LoadScene("Gameplay");
        }

        private void ShowMenu()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }

            Debug.Log("[MainMenuUI] Menu shown.");
        }

        private void HideMenu()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            Debug.Log("[MainMenuUI] Menu hidden.");
        }

        private void ValidateReferences()
        {
            if (classicModeButton == null)
                Debug.LogWarning("[MainMenuUI] ClassicModeButton not assigned!");

            if (timeAttackModeButton == null)
                Debug.LogWarning("[MainMenuUI] TimeAttackModeButton not assigned!");

            if (_session == null)
                Debug.LogError("[MainMenuUI] GameSessionController.Instance is null! Make sure it exists in the scene.");

            if (menuPanel == null)
                Debug.LogWarning("[MainMenuUI] MenuPanel not assigned - menu won't show/hide!");
        }
    }
}