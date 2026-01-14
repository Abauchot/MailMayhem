using System;
using System.Collections;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Letter
{
    public class LetterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject letterPrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private Sprite[] symbolSprites;
        [SerializeField] private float delayAfterResolve = 0.5f;
        [SerializeField] private HitResolver hitResolver;

        private Letter _currentLetter;
        private Coroutine _spawnDelayCoroutine;
        private GameSessionController _session;

        private void Start()
        {
            if (letterPrefab == null)
            {
                Debug.LogError($"[LetterSpawner] Missing required reference: letterPrefab on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            if (spawnPosition == null)
            {
                Debug.LogError($"[LetterSpawner] Missing required reference: spawnPosition on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            if (hitResolver == null)
            {
                Debug.LogError($"[LetterSpawner] Missing required reference: hitResolver on '{gameObject.name}'. Disabling.");
                enabled = false;
                return;
            }

            _session = GameSessionController.Instance;
            if (_session == null)
            {
                Debug.LogError($"[LetterSpawner] GameSessionController.Instance is null in Start(). Disabling.");
                enabled = false;
                return;
            }

            _session.OnStateChanged += HandleStateChanged;
            hitResolver.OnLetterResolved += HandleLetterResolved;

            // Sync with current state
            HandleStateChanged(_session.CurrentState);
        }

        private void OnDisable()
        {
            StopSpawnDelay();
        }

        private void OnDestroy()
        {
            if (_session != null)
            {
                _session.OnStateChanged -= HandleStateChanged;
            }

            if (hitResolver != null)
            {
                hitResolver.OnLetterResolved -= HandleLetterResolved;
            }
        }

        private void HandleStateChanged(GameSessionController.SessionState state)
        {
            switch (state)
            {
                case GameSessionController.SessionState.Idle:
                    StopSpawnDelay();
                    ClearCurrentLetter();
                    break;
                case GameSessionController.SessionState.Playing:
                    SpawnImmediate();
                    break;
                case GameSessionController.SessionState.GameOver:
                    StopSpawnDelay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void HandleLetterResolved(Letter letter, Boxes.ServiceBox box, DeliveryResult result)
        {
            if (letter != _currentLetter) return;

            Destroy(_currentLetter.gameObject);
            _currentLetter = null;

            if (_session.CurrentState == GameSessionController.SessionState.Playing)
            {
                _spawnDelayCoroutine = StartCoroutine(SpawnAfterDelay());
            }
        }

        private IEnumerator SpawnAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterResolve);

            if (_session.CurrentState == GameSessionController.SessionState.Playing)
            {
                SpawnImmediate();
            }
        }

        private void SpawnImmediate()
        {
            if (_currentLetter != null) return;

            int randomIndex = Random.Range(0, symbolSprites.Length);
            SymbolType randomSymbol = (SymbolType)randomIndex;
            Sprite randomSprite = symbolSprites[randomIndex];

            Debug.Log($"Spawning letter with symbol: {randomSymbol}");

            GameObject instance = Instantiate(letterPrefab, spawnPosition.position, spawnPosition.rotation);
            Letter letter = instance.GetComponent<Letter>();

            if (!letter)
            {
                Debug.LogError("LetterSpawner: letterPrefab does not have a Letter component!");
                Destroy(instance);
                return;
            }

            letter.Initialize(randomSymbol, randomSprite, hitResolver);
            _currentLetter = letter;
        }

        private void StopSpawnDelay()
        {
            if (_spawnDelayCoroutine == null) return;
            StopCoroutine(_spawnDelayCoroutine);
            _spawnDelayCoroutine = null;
        }

        private void ClearCurrentLetter()
        {
            if (_currentLetter == null) return;
            Destroy(_currentLetter.gameObject);
            _currentLetter = null;
        }
    }
}
