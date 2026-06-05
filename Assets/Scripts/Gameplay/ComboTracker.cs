using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ComboTracker : MonoBehaviour
    {
        [SerializeField] private SwipeInput swipeInput;
        [SerializeField, Min(1)] private int maxMultiplier = 5;
        [SerializeField, Min(2)] private int energisedThreshold = 3;

        public int CurrentMultiplier => _currentMultiplier;

        public event Action OnEnergisedEntered;
        public event Action OnStrokeEnded;
        public event Action<int> OnMultiplierChanged;

        private int _slicesThisStroke;
        private int _currentMultiplier = 1;
        private bool _energisedThisStroke;

        private void Awake()
        {
            if(swipeInput == null)
            {
                swipeInput = GetComponent<SwipeInput>();
            }

            if(swipeInput == null)
            {
                Debug.LogError("[ComboTracker] No SwipeInput reference assigned - disabling.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if(swipeInput == null)
            {
                return;
            }
            swipeInput.OnPressStarted += HandlePressStarted;
            swipeInput.OnPressReleased += HandlePressReleased;
        }

        private void OnDisable()
        {
            if(swipeInput == null)
            {
                return;
            }
            swipeInput.OnPressStarted -= HandlePressStarted;
            swipeInput.OnPressReleased -= HandlePressReleased;
        }

        public int RegisterSlice()
        {
            _slicesThisStroke++;

            int newMultiplier = Mathf.Min(_slicesThisStroke, maxMultiplier);

            if(newMultiplier != _currentMultiplier)
            {
                _currentMultiplier = newMultiplier;
                OnMultiplierChanged?.Invoke(_currentMultiplier);
            }

            if(_currentMultiplier >= energisedThreshold && !_energisedThisStroke)
            {
                _energisedThisStroke = true;
                OnEnergisedEntered?.Invoke();
            }

            return _currentMultiplier;
        }

        private void HandlePressStarted()
        {
            ResetStroke();
        }

        private void HandlePressReleased()
        {
            if(_energisedThisStroke)
            {
                OnStrokeEnded?.Invoke();
            }

            ResetStroke();
        }

        private void ResetStroke()
        {
            _slicesThisStroke = 0;
            _currentMultiplier = 1;
            _energisedThisStroke = false;
            OnMultiplierChanged?.Invoke(_currentMultiplier);
        }
    }
}
