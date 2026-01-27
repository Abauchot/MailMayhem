using UnityEngine;

namespace Gameplay.Difficulty
{
    /// <summary>
    /// Defines difficulty progression parameters for a game mode.
    /// Create instances via Assets > Create > Mail Mayhem > Difficulty Settings.
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultySettings", menuName = "Mail Mayhem/Difficulty Settings", order = 1)]
    public class DifficultySettings : ScriptableObject
    {
        [Header("Spawn Rate Progression")]
        [Tooltip("Initial delay between letter spawns (seconds)")]
        public float baseSpawnDelay = 2.0f;
        
        [Tooltip("Minimum spawn delay (speed cap)")]
        public float minSpawnDelay = 0.5f;
        
        [Tooltip("How much to reduce delay per level")]
        public float spawnDelayDecrement = 0.1f;
        
        [Tooltip("Score required to increase difficulty level")]
        public int scorePerLevel = 500;

        [Header("Box Permutation")]
        [Tooltip("Enable box position swapping")]
        public bool enableBoxPermutation = true;
        
        [Tooltip("Initial delay before first permutation")]
        public float initialPermutationDelay = 30f;
        
        [Tooltip("Time between permutations")]
        public float permutationInterval = 20f;
        
        [Tooltip("Minimum interval as difficulty increases")]
        public float minPermutationInterval = 8f;
        
        [Tooltip("How much to reduce interval per level")]
        public float permutationIntervalDecrement = 1f;

        [Header("Combo Pressure (Optional)")]
        [Tooltip("Enable time pressure on high combos")]
        public bool enableComboPressure = false;
        
        [Tooltip("Combo threshold to start time pressure")]
        public int comboPressureThreshold = 10;
        
        [Tooltip("Extra spawn delay reduction when above threshold")]
        public float comboPressureMultiplier = 0.8f;

        /// <summary>
        /// Calculate current spawn delay based on difficulty level.
        /// </summary>
        public float GetSpawnDelay(int difficultyLevel)
        {
            float delay = baseSpawnDelay - (spawnDelayDecrement * difficultyLevel);
            return Mathf.Max(delay, minSpawnDelay);
        }

        /// <summary>
        /// Calculate current permutation interval based on difficulty level.
        /// </summary>
        public float GetPermutationInterval(int difficultyLevel)
        {
            if (!enableBoxPermutation) return float.MaxValue;
            
            float interval = permutationInterval - (permutationIntervalDecrement * difficultyLevel);
            return Mathf.Max(interval, minPermutationInterval);
        }

        /// <summary>
        /// Calculate difficulty level from score.
        /// </summary>
        public int GetDifficultyLevel(int score)
        {
            return Mathf.FloorToInt(score / (float)scorePerLevel);
        }
    }
}