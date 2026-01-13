using Gameplay.Boxes;
using UnityEngine;

namespace Gameplay.Letter
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Letter : MonoBehaviour
    {
        private SymbolType _symbol;
        private bool _isResolved;
        private SpriteRenderer _spriteRenderer;
        private HitResolver _hitResolver;

        public SymbolType Symbol => _symbol;
        public bool IsResolved => _isResolved;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(SymbolType symbol, Sprite sprite, HitResolver hitResolver)
        {
            _symbol = symbol;
            _hitResolver = hitResolver;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isResolved) return;

            var box = other.GetComponent<ServiceBox>();
            if (box == null) return;

            if (_hitResolver == null)
            {
                Debug.LogWarning("Letter: HitResolver is null (not injected).");
                return;
            }

            _hitResolver.Resolve(this, box);
        }
    }
}
