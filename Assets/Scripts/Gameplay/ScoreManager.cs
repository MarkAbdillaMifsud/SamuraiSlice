using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ScoreManager : MonoBehaviour
    {
        const int BaseSlicePoints = 5;

        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

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
            CurrentScore += BaseSlicePoints;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
