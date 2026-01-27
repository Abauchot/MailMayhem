using System;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the Main Menu UI.
    /// Handles Start Game button and displays game title.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button startGameButton;

        [Header("UI Elements")]
        [SerializeField] private GameObject menuPanel;

        private GameSessionController _session;

        private void Start()
        {
            _session = GameSessionController.Instance;

            ValidateReferences();

            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(OnStartGameClicked);
            }

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
            if (startGameButton != null)
            {
                startGameButton.onClick.RemoveListener(OnStartGameClicked);
            }

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void OnStartGameClicked()
        {
            Debug.Log("[MainMenuUI] Start Game button clicked.");

            if (_session != null)
            {
                _session.StartGame();
            }
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
            if (startGameButton == null)
                Debug.LogWarning("[MainMenuUI] StartGameButton not assigned!");

            if (_session == null)
                Debug.LogError("[MainMenuUI] GameSessionController.Instance is null! Make sure it exists in the scene.");

            if (menuPanel == null)
                Debug.LogWarning("[MainMenuUI] MenuPanel not assigned - menu won't show/hide!");
        }
    }
}