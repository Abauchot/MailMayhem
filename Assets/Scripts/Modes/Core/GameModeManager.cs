using UI;
using UnityEngine;

namespace Modes.Core
{
    /// <summary>
    /// Activates only the selected game mode in the Gameplay scene.
    /// Runs in Awake() so the non-selected mode's Start() never executes.
    /// </summary>
    public class GameModeManager : MonoBehaviour
    {
        [Header("Mode References")]
        [SerializeField] private GameMode classicMode;
        [SerializeField] private GameMode timeAttackMode;

        [Header("UI")]
        [SerializeField] private ModeStatusUI modeStatusUI;

        private void Awake()
        {
            GameMode selected;
            GameMode disabled;

            switch (GameModeSelection.SelectedMode)
            {
                case GameModeType.TimeAttack:
                    selected = timeAttackMode;
                    disabled = classicMode;
                    break;
                default:
                    selected = classicMode;
                    disabled = timeAttackMode;
                    break;
            }

            if (disabled != null)
            {
                disabled.gameObject.SetActive(false);
                Debug.Log($"[GameModeManager] Disabled {disabled.ModeName} mode.");
            }

            if (modeStatusUI != null && selected != null)
            {
                modeStatusUI.SetActiveMode(selected);
            }

            Debug.Log($"[GameModeManager] Active mode: {GameModeSelection.SelectedMode}");
        }
    }
}
