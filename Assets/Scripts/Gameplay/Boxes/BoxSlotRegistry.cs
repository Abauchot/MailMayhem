using UnityEngine;

namespace Gameplay.Boxes
{
    public class BoxSlotRegistry : MonoBehaviour
    {
        // Removed Singleton Instance

        [SerializeField] private ServiceBox[] slots = new ServiceBox[4];

        public int SlotCount => slots.Length;

        // Removed Awake


        public ServiceBox GetSlot(int index)
        {
            if (index < 0 || index >= slots.Length)
            {
                Debug.LogWarning($"BoxSlotRegistry: Invalid slot index {index}");
                return null;
            }
            return slots[index];
        }
    }
}
