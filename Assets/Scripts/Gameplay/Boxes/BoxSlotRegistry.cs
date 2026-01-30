using UnityEngine;

namespace Gameplay.Boxes
{
    /// <summary>
    /// Registry that maps slot indices to ServiceBox instances.
    /// Supports dynamic remapping during box permutations.
    /// </summary>
    public class BoxSlotRegistry : MonoBehaviour
    {
        [SerializeField] private ServiceBox[] slots;

        public int SlotCount => slots?.Length ?? 0;

        /// <summary>
        /// Get the ServiceBox currently assigned to the given slot index.
        /// </summary>
        public ServiceBox GetSlot(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length)
            {
                Debug.LogWarning($"[BoxSlotRegistry] Invalid slot index: {index}");
                return null;
            }

            return slots[index];
        }

        /// <summary>
        /// Update the slot mapping with a new array of boxes.
        /// Called by BoxPermutationController after shuffling.
        /// </summary>
        public void UpdateSlotMapping(ServiceBox[] newMapping)
        {
            if (newMapping == null)
            {
                Debug.LogError("[BoxSlotRegistry] Cannot update with null mapping!");
                return;
            }

            if (newMapping.Length != slots.Length)
            {
                Debug.LogError($"[BoxSlotRegistry] Mapping length mismatch! Expected {slots.Length}, got {newMapping.Length}");
                return;
            }

            slots = newMapping;

            Debug.Log("[BoxSlotRegistry] Slot mapping updated:");
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    Debug.Log($"  Slot {i} → {slots[i].AcceptedSymbol}");
                }
            }
        }

        private void OnValidate()
        {
            if (slots == null || slots.Length == 0)
            {
                Debug.LogWarning("[BoxSlotRegistry] Slots array is empty! Assign ServiceBox references in Inspector.");
            }
        }
    }
}