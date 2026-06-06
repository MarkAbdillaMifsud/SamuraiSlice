using TMPro;
using UnityEngine;

namespace SamuraiSlice
{
    public class SwipeDebugHud : MonoBehaviour
    {
        [SerializeField] private SwipeInput swipeInput;
        [SerializeField] private TMP_Text debugText;

        private void Update()
        {
            if (swipeInput == null || debugText == null)
            {
                return;
            }

            bool hasSegment = swipeInput.TryGetCurrentSegment(out Vector2 from, out Vector2 to);
            float delta = Vector2.Distance(from, to);

            debugText.text =
                $"Swiping: {swipeInput.IsSwiping}\n" +
                $"Segment: {hasSegment}\n" +
                $"Delta: {delta:0.0000}\n" +
                $"Current: {swipeInput.CurrentWorldPoint}\n" +
                $"From: {from}\n" +
                $"To: {to}\n" +
                $"Screen: {Screen.width}x{Screen.height}";
        }
    }
}