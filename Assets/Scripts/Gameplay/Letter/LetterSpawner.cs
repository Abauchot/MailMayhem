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

        private void OnEnable()
        {
            if (GameSessionController.Instance == null)
            {
                Debug.LogWarning("LetterSpawner: GameSessionController Instance is null.");
                return;
            }

            GameSessionController.Instance.OnStateChanged += HandleStateChanged;

            if (hitResolver == null)
            {
                Debug.LogError("LetterSpawner: HitResolver reference is missing!");
                enabled = false;
                return;
            }

            hitResolver.OnLetterResolved += HandleLetterResolved;

            HandleStateChanged(GameSessionController.Instance.CurrentState);
        }

        private void OnDisable()
        {
            StopSpawnDelay();

            if (GameSessionController.Instance != null)
            {
                GameSessionController.Instance.OnStateChanged -= HandleStateChanged;
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

            if (GameSessionController.Instance != null &&
                GameSessionController.Instance.CurrentState == GameSessionController.SessionState.Playing)
            {
                _spawnDelayCoroutine = StartCoroutine(SpawnAfterDelay());
            }
        }

        private IEnumerator SpawnAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterResolve);

            if (GameSessionController.Instance != null &&
                GameSessionController.Instance.CurrentState == GameSessionController.SessionState.Playing)
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
