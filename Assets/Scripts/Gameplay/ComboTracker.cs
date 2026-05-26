using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ComboTracker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SwipeInput swipeInput;

        [Header("Tuning")]
        [SerializeField, Min(1)] private int maxMultiplier = 5;
        [SerializeField, Min(2)] private int energisedThreshold = 3;

        public int CurrentMultiplier => Mathf.Min(strokeSliceEvents + 1, maxMultiplier);

        public event Action OnEnergisedEntered;
        public event Action OnStrokeEnded;

        private int strokeSliceEvents;
        private int? frameLockMultiplier;
        private bool energisedThisStroke;

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
            if(frameLockMultiplier == null)
            {
                frameLockMultiplier = CurrentMultiplier;
            }

            int multiplier = frameLockMultiplier.Value;

            if(multiplier >= energisedThreshold && !energisedThisStroke)
            {
                energisedThisStroke = true;
                OnEnergisedEntered?.Invoke();
            }

            Debug.Log($"[Combo] slice mult={multiplier} strokeEvents={strokeSliceEvents} frame={Time.frameCount}");

            return multiplier;
        }

        private void LateUpdate()
        {
            if(frameLockMultiplier == null)
            {
                return;
            }
            strokeSliceEvents += 1;
            frameLockMultiplier = null;
        }

        private void HandlePressStarted()
        {
            strokeSliceEvents = 0;
            frameLockMultiplier = null;
            energisedThisStroke = false;
        }

        private void HandlePressReleased()
        {
            if(energisedThisStroke)
            {
                OnStrokeEnded?.Invoke();
            }
        }
    }
}
