using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SamuraiSlice
{
    public class CharacterSlot : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [SerializeField] private TMP_Text characterText;
        [SerializeField] private float pixelsPerStep = 30f;

        private int _index;
        private float _dragAccumulator;

        public char CurrentChar => Alphabet[_index];

        private void Awake()
        {
            _index = 0;
            Refresh();
        }

        public void SetChar(char c)
        {
            int idx = Alphabet.IndexOf(char.ToUpper(c));
            _index = idx >= 0 ? idx : 0;
            Refresh();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragAccumulator = 0f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragAccumulator += eventData.delta.y;

            while(_dragAccumulator >= pixelsPerStep)
            {
                Step(1);
                _dragAccumulator -= pixelsPerStep;
            }

            while(_dragAccumulator <=  pixelsPerStep)
            {
                Step(-1);
                _dragAccumulator += pixelsPerStep;
            }
        }

        private void Step(int direction)
        {
            _index = (_index + direction + Alphabet.Length) % Alphabet.Length;
            Refresh();
        }

        private void Refresh()
        {
            if(characterText != null)
            {
                characterText.text = CurrentChar.ToString();
            }
        }
    }
}
