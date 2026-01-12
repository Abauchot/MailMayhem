using UnityEngine;

namespace Gameplay.Boxes
{
    public class ServiceBox : MonoBehaviour
    {
        [SerializeField] private SymbolType acceptedSymbol;

        public SymbolType AcceptedSymbol => acceptedSymbol;
    }
}
