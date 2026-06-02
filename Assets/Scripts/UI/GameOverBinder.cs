using TMPro;
using UnityEngine;

namespace SamuraiSlice
{
    public class GameOverBinder : MonoBehaviour
    {
        [SerializeField] private LeaderboardManager leaderboardManager;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text[] entryTexts;

        private int _cachedLastScore;

        private void Awake()
        {
            _cachedLastScore = PlayerPrefs.GetInt("SamuraiSlice_LastScore", 0);
        }

        private void Start()
        {
            PopulateFinalScore();
        }

        public void ShowLeaderboard()
        {
            if(leaderboardManager == null)
            {
                Debug.LogWarning("[GameOverBinder] LeaderboardManager not assigned.", this);
                return;
            }

            PopulateLeaderboard();
        }

        private void PopulateFinalScore()
        {
            if(finalScoreText != null)
            {
                finalScoreText.text = FormatScore(_cachedLastScore);
            }
        }

        private void PopulateLeaderboard()
        {
            var entries = leaderboardManager.Entries;

            for (int i = 0; i < entryTexts.Length; i++)
            {
                if (entryTexts[i] == null)
                {
                    continue;
                }

                if (i < entries.Count)
                {
                    entryTexts[i].gameObject.SetActive(true);
                    entryTexts[i].text = $"{i + 1}. {entries[i].name} - {FormatScore(entries[i].score)}";
                } else
                {
                    entryTexts[i].gameObject.SetActive(false);
                }
            }
        }

        private static string FormatScore(int score)
        {
            return score.ToString("#,##0");
        }
    }
}
