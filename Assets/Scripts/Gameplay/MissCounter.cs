using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class MissCounter : MonoBehaviour
    {
        [SerializeField] private int maxMisses = 3;
        public int CurrentMisses { get; private set; }
        public static event Action<int> OnMissCountChanged;
        public event Action OnMissThresholdReached;

        private void OnEnable()
        {
            Ingredient.OnIngredientMissed += HandleMiss;
        }

        private void OnDisable()
        {
            Ingredient.OnIngredientMissed -= HandleMiss;
        }

        private void HandleMiss()
        {
            if(CurrentMisses >= maxMisses)
            {
                return;
            }

            CurrentMisses++;
            Debug.Log($"[MissCounter] Miss {CurrentMisses}/{maxMisses}");
            OnMissCountChanged?.Invoke( CurrentMisses);
            if(CurrentMisses >= maxMisses)
            {
                OnMissThresholdReached?.Invoke();
            }
        }

        public void ResetCount()
        {
            CurrentMisses = 0;
            OnMissCountChanged?.Invoke(CurrentMisses);
        }
    }
}
