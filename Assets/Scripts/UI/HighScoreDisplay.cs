using UnityEngine;
using TMPro;

namespace SamuraiSlice
{
    public class HighScoreDisplay : MonoBehaviour
    {
        [SerializeField] private HighScoreManager highScoreManager;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private GameObject newRecordLabel;

        private void OnEnable()
        {
            if (highScoreManager == null)
            {
                return;
            }

            highScoreManager.OnHighScoreChanged += HandleHighScoreChanged;
            highScoreManager.OnNewHighScore += HandleNewHighScore;

            HandleHighScoreChanged(highScoreManager.HighScore);
        }

        private void OnDisable()
        {
            if (highScoreManager == null)
            {
                return;
            }

            highScoreManager.OnHighScoreChanged -= HandleHighScoreChanged;
            highScoreManager.OnNewHighScore -= HandleNewHighScore;
        }

        private void HandleHighScoreChanged(int newHighScore)
        {
            if (highScoreText != null)
            {
                highScoreText.text = newHighScore.ToString();
            }
        }

        private void HandleNewHighScore(int newHighScore)
        {
            if (newRecordLabel != null)
            {
                newRecordLabel.SetActive(true);
            }
        }
    }
}
