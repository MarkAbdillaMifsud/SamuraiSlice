using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ComboTracker comboTracker;

        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (comboTracker == null)
            {
                Debug.LogError("[ScoreManager] No ComboTracker reference assigned - scores will not be multiplied.", this);
            }
        }

        private void OnEnable()
        {
            Ingredient.Sliced += HandleSliced;
        }

        private void OnDisable()
        {
            Ingredient.Sliced -= HandleSliced;
        }

        private void HandleSliced(Ingredient ingredient)
        {
            int points = ingredient.Data != null ? ingredient.Data.points : 0;
            if (points == 0)
            {
                return;
            }

            int multiplier = comboTracker != null ? comboTracker.RegisterSlice() : 1;

            CurrentScore += points * multiplier;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
