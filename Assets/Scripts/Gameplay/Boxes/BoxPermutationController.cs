using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Difficulty;
using UnityEngine;

namespace Gameplay.Boxes
{
    /// <summary>
    /// MIXED MODE - Combines Mirror, Swap, and Shuffle with visual warnings.
    /// Player gets 1 second warning showing which mode is coming.
    /// </summary>
    public class BoxPermutationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoxSlotRegistry boxRegistry;

        [Header("Mode Settings")]
        [Tooltip("Enable each permutation mode")]
        [SerializeField] private bool enableMirror = true;
        [SerializeField] private bool enableSwap = true;
        [SerializeField] private bool enableShuffle = true;

        [Header("Timing Settings")]
        [Tooltip("Time between permutations (seconds)")]
        [SerializeField] private float permutationInterval = 15f;
        
        [Tooltip("Delay before first permutation (seconds)")]
        [SerializeField] private float initialPermutationDelay = 25f;

        [Tooltip("Warning time before permutation (seconds)")]
        [SerializeField] private float warningDuration = 1.5f;

        [Header("Animation Settings")]
        [SerializeField] private float swapDuration = 0.4f;
        [SerializeField] private Ease swapEase = Ease.InOutQuad;
        [SerializeField] private float anticipationScale = 1.1f;
        [SerializeField] private float anticipationDuration = 0.15f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        private bool _isPermuting;
        private Dictionary<int, Vector3> _slotPositions;
        private bool _initialized;
        private Vector3 _defaultScale = Vector3.one;

        private float _permutationTimer;
        private bool _permutationTimerStarted;
        private int _permutationCount = 0;

        private bool _isMirrored = false; // Track mirror state

        public enum PermutationMode
        {
            Mirror,
            Swap,
            Shuffle
        }

        /// <summary>
        /// Event fired when warning should be displayed.
        /// UI can subscribe to this to show warning overlay.
        /// </summary>
        public event System.Action<PermutationMode, float> OnPermutationWarning;

        /// <summary>
        /// Event fired when permutation completes.
        /// </summary>
        public event System.Action OnPermutationComplete;

        private void Start()
        {
            ValidateReferences();
            InitializeSlotPositions();

            Debug.Log("[BoxPermutation] MIXED MODE initialized");
            Debug.Log($"  Modes: Mirror={enableMirror}, Swap={enableSwap}, Shuffle={enableShuffle}");
            Debug.Log($"  First permutation in: {initialPermutationDelay}s");
            Debug.Log($"  Then every: {permutationInterval}s");
            Debug.Log($"  Warning time: {warningDuration}s");
        }

        private void Update()
        {
            if (!_initialized) return;
            
            UpdatePermutationTimer();
        }

        private void InitializeSlotPositions()
        {
            _slotPositions = new Dictionary<int, Vector3>();
            bool scaleCaptured = false;

            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                ServiceBox box = boxRegistry.GetSlot(i);
                if (box != null)
                {
                    _slotPositions[i] = box.transform.position;
                    
                    if (!scaleCaptured)
                    {
                        _defaultScale = box.transform.localScale;
                        scaleCaptured = true;
                    }
                }
            }

            _initialized = true;
        }

        private void UpdatePermutationTimer()
        {
            if (_isPermuting) return;

            // Initial delay
            if (!_permutationTimerStarted)
            {
                _permutationTimer += Time.deltaTime;
                if (_permutationTimer >= initialPermutationDelay)
                {
                    _permutationTimerStarted = true;
                    _permutationTimer = 0f;
                    TriggerPermutation();
                }
                return;
            }

            // Regular interval
            _permutationTimer += Time.deltaTime;
            if (_permutationTimer >= permutationInterval)
            {
                _permutationTimer = 0f;
                TriggerPermutation();
            }
        }

        private void TriggerPermutation()
        {
            if (_isPermuting) return;

            _permutationCount++;

            // Pick random mode from enabled modes
            PermutationMode mode = GetRandomEnabledMode();

            if (showDebugInfo)
            {
                Debug.Log($"[Permutation] #{_permutationCount} - {mode} mode selected");
            }

            StartCoroutine(ExecutePermutationWithWarning(mode));
        }

        private PermutationMode GetRandomEnabledMode()
        {
            List<PermutationMode> enabledModes = new List<PermutationMode>();

            if (enableMirror) enabledModes.Add(PermutationMode.Mirror);
            if (enableSwap) enabledModes.Add(PermutationMode.Swap);
            if (enableShuffle) enabledModes.Add(PermutationMode.Shuffle);

            if (enabledModes.Count == 0)
            {
                Debug.LogWarning("[Permutation] No modes enabled! Defaulting to Swap.");
                return PermutationMode.Swap;
            }

            return enabledModes[Random.Range(0, enabledModes.Count)];
        }

        private IEnumerator ExecutePermutationWithWarning(PermutationMode mode)
        {
            _isPermuting = true;

            // Fire warning event
            if (showDebugInfo)
            {
                Debug.Log($"[Permutation] ⚠️ WARNING: {mode} incoming in {warningDuration}s!");
            }

            OnPermutationWarning?.Invoke(mode, warningDuration);

            // Wait for warning duration
            yield return new WaitForSeconds(warningDuration);

            // Execute the permutation
            SetBoxCollidersEnabled(false);

            switch (mode)
            {
                case PermutationMode.Mirror:
                    yield return StartCoroutine(ExecuteMirror());
                    break;
                case PermutationMode.Swap:
                    yield return StartCoroutine(ExecuteSwap());
                    break;
                case PermutationMode.Shuffle:
                    yield return StartCoroutine(ExecuteShuffle());
                    break;
            }

            SetBoxCollidersEnabled(true);

            OnPermutationComplete?.Invoke();

            _isPermuting = false;
        }

        private void SetBoxCollidersEnabled(bool enabled)
        {
            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                ServiceBox box = boxRegistry.GetSlot(i);
                if (box != null)
                {
                    var col = box.GetComponent<Collider2D>();
                    if (col != null) col.enabled = enabled;
                }
            }
        }

        #region Mirror Mode
        private IEnumerator ExecuteMirror()
        {
            _isMirrored = !_isMirrored;

            ServiceBox[] currentBoxes = new ServiceBox[boxRegistry.SlotCount];
            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                currentBoxes[i] = boxRegistry.GetSlot(i);
            }

            if (showDebugInfo)
            {
                Debug.Log($"[Mirror] State: {(_isMirrored ? "MIRRORED" : "NORMAL")}");
            }

            // Create mirrored mapping
            ServiceBox[] mirroredBoxes = new ServiceBox[boxRegistry.SlotCount];
            for (int i = 0; i < currentBoxes.Length; i++)
            {
                int mirrorIndex = (currentBoxes.Length - 1) - i;
                mirroredBoxes[mirrorIndex] = currentBoxes[i];
            }

            // Animate boxes
            for (int i = 0; i < currentBoxes.Length; i++)
            {
                if (currentBoxes[i] == null) continue;

                int targetSlot = (currentBoxes.Length - 1) - i;
                Vector3 targetPosition = _slotPositions[targetSlot];

                StartCoroutine(AnimateBoxToPosition(currentBoxes[i], targetPosition, i));
            }

            yield return new WaitForSeconds(anticipationDuration + swapDuration + anticipationDuration);

            boxRegistry.UpdateSlotMapping(mirroredBoxes);
        }
        #endregion

        #region Swap Mode
        private IEnumerator ExecuteSwap()
        {
            ServiceBox[] currentBoxes = new ServiceBox[boxRegistry.SlotCount];
            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                currentBoxes[i] = boxRegistry.GetSlot(i);
            }

            // Pick 2 random slots
            int slot1 = Random.Range(0, boxRegistry.SlotCount);
            int slot2 = Random.Range(0, boxRegistry.SlotCount);
            
            while (slot2 == slot1)
            {
                slot2 = Random.Range(0, boxRegistry.SlotCount);
            }

            ServiceBox box1 = currentBoxes[slot1];
            ServiceBox box2 = currentBoxes[slot2];

            if (showDebugInfo)
            {
                Debug.Log($"[Swap] {box1.AcceptedSymbol} (slot {slot1}) ↔ {box2.AcceptedSymbol} (slot {slot2})");
            }

            // Create swapped mapping
            ServiceBox[] newBoxes = new ServiceBox[boxRegistry.SlotCount];
            for (int i = 0; i < currentBoxes.Length; i++)
            {
                if (i == slot1)
                    newBoxes[i] = box2;
                else if (i == slot2)
                    newBoxes[i] = box1;
                else
                    newBoxes[i] = currentBoxes[i];
            }

            // Animate the 2 boxes
            StartCoroutine(AnimateBoxToPosition(box1, _slotPositions[slot2], 0));
            StartCoroutine(AnimateBoxToPosition(box2, _slotPositions[slot1], 1));

            yield return new WaitForSeconds(anticipationDuration + swapDuration + anticipationDuration);

            boxRegistry.UpdateSlotMapping(newBoxes);
        }
        #endregion

        #region Shuffle Mode
        private IEnumerator ExecuteShuffle()
        {
            ServiceBox[] currentBoxes = new ServiceBox[boxRegistry.SlotCount];
            List<int> slotIndices = new List<int>();
            
            for (int i = 0; i < boxRegistry.SlotCount; i++)
            {
                currentBoxes[i] = boxRegistry.GetSlot(i);
                slotIndices.Add(i);
            }

            // Shuffle slots
            List<int> shuffledSlots = new List<int>(slotIndices);
            ShuffleList(shuffledSlots);

            // Ensure at least one box moves
            bool anyMoved = false;
            for (int i = 0; i < slotIndices.Count; i++)
            {
                if (slotIndices[i] != shuffledSlots[i])
                {
                    anyMoved = true;
                    break;
                }
            }

            if (!anyMoved && slotIndices.Count >= 2)
            {
                (shuffledSlots[0], shuffledSlots[1]) = (shuffledSlots[1], shuffledSlots[0]);
            }

            if (showDebugInfo)
            {
                Debug.Log("[Shuffle] Complete randomization");
            }

            // Create shuffled mapping
            ServiceBox[] shuffledBoxes = new ServiceBox[boxRegistry.SlotCount];
            for (int i = 0; i < slotIndices.Count; i++)
            {
                int originalSlot = slotIndices[i];
                int targetSlot = shuffledSlots[i];
                shuffledBoxes[targetSlot] = currentBoxes[originalSlot];
            }

            // Animate boxes
            for (int i = 0; i < slotIndices.Count; i++)
            {
                int originalSlot = slotIndices[i];
                int targetSlot = shuffledSlots[i];
                ServiceBox box = currentBoxes[originalSlot];
                Vector3 targetPosition = _slotPositions[targetSlot];

                StartCoroutine(AnimateBoxToPosition(box, targetPosition, i));
            }

            yield return new WaitForSeconds(anticipationDuration + swapDuration + anticipationDuration);

            boxRegistry.UpdateSlotMapping(shuffledBoxes);
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        #endregion

        private IEnumerator AnimateBoxToPosition(ServiceBox box, Vector3 targetPosition, int animationIndex)
        {
            Vector3 baseScale = _defaultScale;
            float referenceZ = targetPosition.z;
            float zOffset = -animationIndex * 0.1f;

            box.transform.DOKill();

            Vector3 currentPos = box.transform.position;
            Vector3 startPos = new Vector3(currentPos.x, currentPos.y, referenceZ + zOffset);
            box.transform.position = startPos;
            
            box.transform.DOScale(baseScale * anticipationScale, anticipationDuration);
            yield return new WaitForSeconds(anticipationDuration);

            Vector3 targetWithOffset = new Vector3(targetPosition.x, targetPosition.y, referenceZ + zOffset);
            box.transform.DOMove(targetWithOffset, swapDuration).SetEase(swapEase);
            yield return new WaitForSeconds(swapDuration);

            box.transform.DOScale(baseScale, anticipationDuration);
            yield return new WaitForSeconds(anticipationDuration);

            box.transform.localScale = baseScale;
            box.transform.position = new Vector3(targetPosition.x, targetPosition.y, referenceZ);
        }

        private void ValidateReferences()
        {
            if (boxRegistry == null)
                Debug.LogError("[Permutation] BoxSlotRegistry reference missing!");
        }

        public int PermutationCount => _permutationCount;
        public bool IsMirrored => _isMirrored;

        public float TimeUntilNextPermutation
        {
            get
            {
                if (!_permutationTimerStarted)
                    return initialPermutationDelay - _permutationTimer;
                else
                    return permutationInterval - _permutationTimer;
            }
        }
    }
}