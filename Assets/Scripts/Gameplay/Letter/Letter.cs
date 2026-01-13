using UnityEngine;

namespace Gameplay.Letter
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Letter : MonoBehaviour
    {
        private SymbolType _symbol;
        private bool _isResolved;
        private SpriteRenderer _spriteRenderer;

        public SymbolType Symbol => _symbol;
        public bool IsResolved => _isResolved;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(SymbolType symbol, Sprite sprite)
        {
            _symbol = symbol;

            if (!_spriteRenderer)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            _spriteRenderer.sprite = sprite;
            Debug.Log($"Letter.Initialize: {symbol} sprite={sprite?.name}", this);
        }

        public void MarkAsResolved()
        {
            _isResolved = true;
        }
    }
}
