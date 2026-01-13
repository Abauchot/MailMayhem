using System;
using Core;
using Gameplay.Boxes;
using Gameplay.Letter;
using UnityEngine;

namespace Gameplay
{
    public class HitResolver : MonoBehaviour
    {
        // Removed Singleton Instance

        public event Action<Letter.Letter, ServiceBox, DeliveryResult> OnLetterResolved;

        // Removed Awake


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

            var result = letter.Symbol == box.AcceptedSymbol
                ? DeliveryResult.Correct
                : DeliveryResult.Error;

            Debug.Log($"HitResolver: {letter.Symbol} -> {box.AcceptedSymbol} = {result}");

            OnLetterResolved?.Invoke(letter, box, result);

            return result;
        }
    }
}
