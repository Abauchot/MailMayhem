using Scoring;
using UnityEngine;

namespace VFX.Score
{
    /// <summary>
    /// Spawns score popups in response to scoring events.
    /// Listens to ScoreSystem and creates popup instances at box positions.
    /// </summary>
    public class ScorePopupSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private GameObject scorePopupPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

        private void Start()
        {
            ValidateReferences();
            scoreSystem.OnScoringEvent += HandleScoringEvent;
            Debug.Log("[ScorePopupSpawner] Initialized and subscribed to scoring events.");
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoringEvent -= HandleScoringEvent;
            }
            Debug.Log("[ScorePopupSpawner] Unsubscribed from scoring events.");
        }

        private void HandleScoringEvent(ScoringEvent evt)
        {
            // Only spawn popups on correct hits with points
            if (!evt.IsCorrect || evt.PointsDelta <= 0)
            {
                return;
            }

            if (evt.HitBox == null)
            {
                Debug.LogWarning("[ScorePopupSpawner] ScoringEvent has null HitBox reference.");
                return;
            }

            SpawnPopup(evt);
        }

        private void SpawnPopup(ScoringEvent evt)
        {
            Vector3 spawnPosition = evt.HitBox.transform.position + spawnOffset;
            
            GameObject popupObj = Instantiate(scorePopupPrefab, spawnPosition, Quaternion.identity);
            
            ScorePopup popup = popupObj.GetComponent<ScorePopup>();
            if (popup == null)
            {
                Debug.LogError("[ScorePopupSpawner] ScorePopup component missing on prefab!");
                Destroy(popupObj);
                return;
            }

            popup.Play(evt.PointsDelta, evt.NewCombo);

            Debug.Log($"[ScorePopupSpawner] Spawned popup: +{evt.PointsDelta} at combo x{evt.NewCombo}");
        }

        private void ValidateReferences()
        {
            if (scoreSystem == null)
                Debug.LogError("[ScorePopupSpawner] ScoreSystem reference missing!");

            if (scorePopupPrefab == null)
                Debug.LogError("[ScorePopupSpawner] ScorePopup prefab reference missing!");
        }
    }
}