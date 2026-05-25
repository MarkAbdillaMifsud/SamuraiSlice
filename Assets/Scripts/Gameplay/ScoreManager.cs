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
            int points = ingredient.Data != null ? ingredient.Data.points : 0;
            if (points == 0)
            {
                return;
            }

            CurrentScore += BaseSlicePoints;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
