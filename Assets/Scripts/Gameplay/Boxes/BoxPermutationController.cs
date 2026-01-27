using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gameplay.Boxes;
using Gameplay.Difficulty;
using UnityEngine;

namespace Gameplay.Boxes
{
    /// <summary>
    /// Handles dynamic box position permutations during gameplay.
    /// Swaps box positions to break muscle memory and increase difficulty.
    /// </summary>
    public class BoxPermutationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DifficultyManager difficultyManager;
        [SerializeField] private BoxSlotRegistry boxRegistry;

        [Header("Animation Settings")]
        [SerializeField] private float swapDuration = 0.4f;
        [SerializeField] private Ease swapEase = Ease.InOutQuad;
        [SerializeField] private float anticipationScale = 1.1f;
        [SerializeField] private float anticipationDuration = 0.15f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        private bool _isPermuting;

        private void Start()
        {
            ValidateReferences();

            if (difficultyManager == null)
            {
                return;
            }
            difficultyManager.OnBoxPermutationTriggered += HandlePermutationTriggered;
            Debug.Log("[BoxPermutationController] Subscribed to permutation events.");
        }

        private void OnDestroy()
        {
            if (difficultyManager != null)
            {
                difficultyManager.OnBoxPermutationTriggered -= HandlePermutationTriggered;
            }
        }

        private void HandlePermutationTriggered()
        {
            if (_isPermuting) return;

            StartCoroutine(PermuteBoxes());
        }

        private IEnumerator PermuteBoxes()
        {
            _isPermuting = true;

            // Get all boxes
            List<ServiceBox> boxes = new List<ServiceBox>();
            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                ServiceBox box = boxRegistry.GetSlot(i);
                if (box)
                {
                    boxes.Add(box);
                }
            }

            if (boxes.Count < 2)
            {
                Debug.LogWarning("[BoxPermutationController] Not enough boxes to permute!");
                _isPermuting = false;
                yield break;
            }

            // Store current positions
            Dictionary<ServiceBox, Vector3> originalPositions = new Dictionary<ServiceBox, Vector3>();
            foreach (ServiceBox box in boxes)
            {
                originalPositions[box] = box.transform.position;
            }

            // Shuffle positions
            List<Vector3> shuffledPositions = new List<Vector3>(originalPositions.Values);
            ShuffleList(shuffledPositions);

            // Ensure at least one box actually moves
            bool anyMoved = boxes.Where((t, i) => Vector3.Distance(originalPositions[t], shuffledPositions[i]) > 0.01f).Any();

            if (!anyMoved)
            {
                // Force a swap between first two boxes
                (shuffledPositions[0], shuffledPositions[1]) = (shuffledPositions[1], shuffledPositions[0]);
            }

            if (showDebugInfo)
            {
                Debug.Log("[BoxPermutationController] Starting box permutation animation...");
            }

            // Animate all boxes simultaneously
            Sequence permutationSequence = DOTween.Sequence();

            for (int i = 0; i < boxes.Count; i++)
            {
                ServiceBox box = boxes[i];
                Vector3 targetPosition = shuffledPositions[i];

                // Skip if already at target
                if (Vector3.Distance(box.transform.position, targetPosition) < 0.01f)
                    continue;

                // Anticipation: scale up slightly
                Tween anticipation = box.transform.DOScale(Vector3.one * anticipationScale, anticipationDuration);
                
                // Move to new position
                Tween move = box.transform.DOMove(targetPosition, swapDuration).SetEase(swapEase);
                
                // Scale back to normal
                Tween scaleBack = box.transform.DOScale(Vector3.one, anticipationDuration);

                // Add to sequence (all boxes move simultaneously)
                if (i == 0)
                {
                    permutationSequence.Append(anticipation);
                    permutationSequence.Append(move);
                    permutationSequence.Append(scaleBack);
                }
                else
                {
                    permutationSequence.Join(anticipation);
                    permutationSequence.Join(move);
                    permutationSequence.Join(scaleBack);
                }
            }

            // Wait for animation to complete
            yield return permutationSequence.WaitForCompletion();

            if (showDebugInfo)
            {
                Debug.Log("[BoxPermutationController] Box permutation complete!");
            }

            _isPermuting = false;
        }

        /// <summary>
        /// Fisher-Yates shuffle algorithm.
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private void ValidateReferences()
        {
            if (difficultyManager == null)
                Debug.LogError("[BoxPermutationController] DifficultyManager reference missing!");

            if (boxRegistry == null)
                Debug.LogError("[BoxPermutationController] BoxSlotRegistry reference missing!");
        }
    }
}