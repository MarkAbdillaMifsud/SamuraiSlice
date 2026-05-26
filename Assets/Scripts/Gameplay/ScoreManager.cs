using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ScoreManager : MonoBehaviour
    {
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

            CurrentScore += ingredient.Data.points;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
