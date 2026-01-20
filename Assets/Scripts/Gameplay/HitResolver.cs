using System;
using Core;
using Gameplay.Boxes;
using Gameplay.Letter;
using UnityEngine;

namespace Gameplay
{
    public class HitResolver : MonoBehaviour
    {
        public event Action<LetterResolution> OnLetterResolved;

        public DeliveryResult Resolve(Letter.Letter letter, ServiceBox box)
        {
            if (GameSessionController.Instance == null ||
                GameSessionController.Instance.CurrentState != GameSessionController.SessionState.Playing)
            {
                Debug.LogWarning("HitResolver: Cannot resolve - session not playing.");
                return DeliveryResult.Error;
            }

            if (letter == null || letter.IsResolved)
            {
                Debug.LogWarning("HitResolver: Cannot resolve - letter null or already resolved.");
                return DeliveryResult.Error;
            }

            if (box == null)
            {
                Debug.LogWarning("HitResolver: Cannot resolve - box is null.");
                return DeliveryResult.Error;
            }

            letter.MarkAsResolved();

            bool isCorrect = letter.Symbol == box.AcceptedSymbol;
            var result = isCorrect ? DeliveryResult.Correct : DeliveryResult.Error;

            Debug.Log($"HitResolver: {letter.Symbol} -> {box.AcceptedSymbol} = {result}");

            var resolution = new LetterResolution(
                letter: letter,
                isCorrect: isCorrect,
                slotIndex: box.SlotIndex,
                expected: box.AcceptedSymbol,
                got: letter.Symbol,
                hitBox: box
            );

            OnLetterResolved?.Invoke(resolution);

            return result;
        }
    }
}
