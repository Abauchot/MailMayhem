using Scoring;
using UnityEngine;

namespace VFX.Combo
{
    /// <summary>
    /// Spawns combo celebration popups at milestone thresholds.
    /// Listens to ScoreSystem and creates dramatic popups at key combo counts.
    /// </summary>
    public class ComboPopupSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private GameObject comboPopupPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int[] comboMilestones = { 3, 5, 10, 15, 20, 25, 30 };

        private int _lastCelebratedCombo = 0;

        private void Start()
        {
            ValidateReferences();
            scoreSystem.OnScoringEvent += HandleScoringEvent;
            Debug.Log("[ComboPopupSpawner] Initialized and subscribed to scoring events.");
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoringEvent -= HandleScoringEvent;
            }
            Debug.Log("[ComboPopupSpawner] Unsubscribed from scoring events.");
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            // Reset tracking when combo breaks
            if (!evt.IsCorrect || evt.NewCombo == 0)
            {
                _lastCelebratedCombo = 0;
                return;
            }

            // Check if we've hit a milestone we haven't celebrated yet
            if (!IsMilestone(evt.NewCombo) || evt.NewCombo <= _lastCelebratedCombo) return;
            SpawnComboPopup(evt.NewCombo);
            _lastCelebratedCombo = evt.NewCombo;
        }

        private bool IsMilestone(int combo)
        {
            foreach (int milestone in comboMilestones)
            {
                if (combo == milestone)
                    return true;
            }
            return false;
        }

        private void SpawnComboPopup(int comboCount)
        {
            // Use spawn point if set, otherwise use a default screen-center position
            Vector3 spawnPosition = spawnPoint != null 
                ? spawnPoint.position 
                : new Vector3(0f, 3f, 0f); 

            GameObject popupObj = Instantiate(comboPopupPrefab, spawnPosition, Quaternion.identity);

            ComboPopup popup = popupObj.GetComponent<ComboPopup>();
            if (popup == null)
            {
                Debug.LogError("[ComboPopupSpawner] ComboPopup component missing on prefab!");
                Destroy(popupObj);
                return;
            }

            popup.Play(comboCount);

            Debug.Log($"[ComboPopupSpawner] Spawned combo milestone popup: x{comboCount}");
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[ComboPopupSpawner] ScoreSystem reference missing!");

            if (comboPopupPrefab == null)
                Debug.LogError("[ComboPopupSpawner] ComboPopup prefab reference missing!");

            if (spawnPoint == null)
                Debug.LogWarning("[ComboPopupSpawner] SpawnPoint not set, using default position.");
        }
    }
}