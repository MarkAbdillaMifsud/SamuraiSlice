using TMPro;
using UnityEngine;

namespace SamuraiSlice
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MissCounter missCounter;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text missText;

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
        }

        private void OnDisable()
        {
            if(scoreManager != null)
            {
                scoreManager.OnScoreChanged -= HandleScoreChanged;
            }
            if (missCounter != null)
            {
                missCounter.OnMissCountChanged -= HandleMissCountChanged;
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
    }
}
