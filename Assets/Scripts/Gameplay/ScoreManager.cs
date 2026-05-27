using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ComboTracker comboTracker;

        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;
        public event Action<int, int, Vector3, Color> OnSliceScored;

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
            int finalPoints = points * multiplier;

            CurrentScore += finalPoints;
            OnScoreChanged?.Invoke(CurrentScore);

            Color accent = ingredient.Data != null ? ingredient.Data.accentColor : Color.white;
            OnSliceScored?.Invoke(finalPoints, multiplier, ingredient.transform.position, accent);
        }
    }
}
