using System;
using UnityEngine;
using System.Collections;
using Core;
using Random = UnityEngine.Random;

namespace Gameplay.Letter
{
    public class LetterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject letterPrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private Sprite[] symbolSprites;
        [SerializeField] private float spawnInterval = 2f;

        private Coroutine _spawnCoroutine;
        private Letter _currentLetter;

        private void OnEnable()
        {
            if (GameSessionController.Instance == null)
            {
                Debug.LogWarning("LetterSpawner: GameSessionController Instance is null.");
                return;
            }

            GameSessionController.Instance.OnStateChanged += HandleStateChanged;
            HandleStateChanged(GameSessionController.Instance.CurrentState);
        }

        private void OnDisable()
        {
            StopSpawning();

            if (GameSessionController.Instance == null) return;

            GameSessionController.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameSessionController.SessionState state)
        {
            switch (state)
            {
                case GameSessionController.SessionState.Idle:
                    StopSpawning();
                    ClearCurrentLetter();
                    break;
                case GameSessionController.SessionState.Playing:
                    StartSpawning();
                    break;
                case GameSessionController.SessionState.GameOver:
                    StopSpawning();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void StartSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        private void StopSpawning()
        {
            if (_spawnCoroutine == null)
            {
                return;
            }
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                // Only spawn if no current letter exists
                if (!_currentLetter)
                {
                    Spawn();
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private Letter Spawn()
        {

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
                return null;
            }

            letter.Initialize(randomSymbol, randomSprite);
            _currentLetter = letter;

            return letter;
        }

        /// <summary>
        /// Clears the current letter (useful for game restart)
        /// </summary>
        private void ClearCurrentLetter()
        {
            if (_currentLetter == null)
            {
                return;
            }
            Destroy(_currentLetter.gameObject);
            _currentLetter = null;
        }

        /// <summary>
        /// Enable or disable spawning
        /// </summary>
        public void SetEnabled(bool isEnabled)
        {
            enabled = isEnabled;
            if (isEnabled)
            {
                return;
            }
            StopSpawning();
            ClearCurrentLetter();
        }
    }
}
