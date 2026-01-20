using Gameplay.Boxes;
using Gameplay.Letter;

namespace Gameplay
{
    /// <summary>
    /// Data emitted when a letter is resolved against a service box.
    /// </summary>
    public readonly struct LetterResolution
    {
        public Letter.Letter Letter { get; }
        public bool IsCorrect { get; }
        public int SlotIndex { get; }
        public SymbolType Expected { get; }
        public SymbolType Got { get; }
        
        public ServiceBox HitBox { get; }

        public LetterResolution(Letter.Letter letter,
            bool isCorrect,
            int slotIndex,
            SymbolType expected,
            SymbolType got, 
            ServiceBox hitBox
            )
        {
            Letter = letter;
            IsCorrect = isCorrect;
            SlotIndex = slotIndex;
            Expected = expected;
            Got = got;
            HitBox = hitBox;
        }
    }
}