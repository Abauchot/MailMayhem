using UnityEngine;

namespace Gameplay.Boxes
{
    public class ServiceBox : MonoBehaviour
    {
        [SerializeField] private SymbolType acceptedSymbol;
        [SerializeField] private int slotIndex = -1;

        public SymbolType AcceptedSymbol => acceptedSymbol;
        public int SlotIndex => slotIndex;
    }
}
