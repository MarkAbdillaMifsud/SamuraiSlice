using TMPro;
using UnityEngine;

namespace SamuraiSlice
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MissCounter missCounter;
        [SerializeField] private ComboTracker comboTracker;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text missText;
        [SerializeField] TMP_Text comboText;

        const int MaxMisses = 3;

        private void OnEnable()
        {
            if(scoreManager != null)
            {
                scoreManager.OnScoreChanged += HandleScoreChanged;
                HandleScoreChanged(scoreManager.CurrentScore);
            }
            if (missCounter != null)
            {
                missCounter.OnMissCountChanged += HandleMissCountChanged;
                HandleMissCountChanged(missCounter.CurrentMisses);
            }
            if (comboTracker != null)
            {
                comboTracker.OnMultiplierChanged += HandleComboChanged;
                HandleComboChanged(comboTracker.CurrentMultiplier);
            }
        }

        private void OnDisable()
        {
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= HandleScoreChanged;
            }
            if (missCounter != null)
            {
                missCounter.OnMissCountChanged -= HandleMissCountChanged;
            }
            if (comboTracker != null)
            {
                comboTracker.OnMultiplierChanged -= HandleComboChanged;
            }
        }

        private void HandleScoreChanged(int newScore)
        {
            if(scoreText != null)
            {
                scoreText.text = newScore.ToString();
            }
        }

        private void HandleMissCountChanged(int newMisses)
        {
            if(missText != null)
            {
                missText.text = $"{newMisses}/{MaxMisses}";
            }
        }

        private void HandleComboChanged(int newMultiplier)
        {
            if(comboText != null)
            {
                comboText.text = newMultiplier <= 1 ? string.Empty : $"x{newMultiplier}";
            }
        }
    }
}
