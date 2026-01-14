using System.Collections;
using Gameplay.Boxes;
using UnityEngine;

namespace Gameplay.Letter
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Letter : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private HitResolver _hitResolver;

        public SymbolType Symbol { get; private set; }

        public bool IsResolved { get; private set; }

        private bool IsArmed { get; set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(SymbolType symbol, Sprite sprite, HitResolver hitResolver)
        {
            Symbol = symbol;
            _hitResolver = hitResolver;
            IsArmed = false;
            IsResolved = false;

            if (!_spriteRenderer)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            _spriteRenderer.sprite = sprite;

            StopAllCoroutines();
            StartCoroutine(ArmNextFrame());
        }

        private IEnumerator ArmNextFrame()
        {
            yield return null;
            IsArmed = true;
            Debug.Log($"[Letter] Armed: {Symbol}", this);
        }

        public void MarkAsResolved()
        {
            IsResolved = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsArmed || IsResolved) return;

            var box = other.GetComponent<ServiceBox>();
            if (box == null) return;

            if (_hitResolver == null)
            {
                Debug.LogWarning("[Letter] HitResolver is null (not injected).");
                return;
            }

            _hitResolver.Resolve(this, box);
        }
    }
}
