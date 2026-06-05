using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSlice
{
    public class GameOverBinder : MonoBehaviour
    {
        [SerializeField] private LeaderboardManager leaderboardManager;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text[] entryTexts;
        [SerializeField] private Image[] rowHighlights;
        [SerializeField] private Color highlightColour = new Color(1f, 0.85f, 0f, 0.45f);
        [SerializeField] private float pulseDuration = 0.35f;


        private int _cachedLastScore;
        private Coroutine _pulseRoutine;

        private void Awake()
        {
            _cachedLastScore = LeaderboardManager.GetLastScore();
        }

        private void Start()
        {
            PopulateFinalScore();
        }

        public void ShowLeaderboard(int newEntryRank = -1)
        {
            if(leaderboardManager == null)
            {
                Debug.LogWarning("[GameOverBinder] LeaderboardManager not assigned.", this);
                return;
            }

            PopulateLeaderboard();

            bool isRankValid = newEntryRank >= 1 && rowHighlights != null && newEntryRank <= rowHighlights.Length && rowHighlights[newEntryRank - 1] != null;

            if (isRankValid) {
                if(_pulseRoutine != null)
                {
                    StopCoroutine( _pulseRoutine );
                }
                _pulseRoutine = StartCoroutine(PulseRow(rowHighlights[newEntryRank - 1]));
            }
            {
                
            }
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

        private IEnumerator PulseRow(Image highlight)
        {
            yield return FadeHighlight(highlight, fromAlpha: 0f, toAlpha: highlightColour.a);

            yield return FadeHighlight(highlight, fromAlpha: highlightColour.a, toAlpha: 0f);

            Color tint = highlightColour;
            tint.a = highlightColour.a * 0.4f;
            highlight.color = tint;
            _pulseRoutine = null;
        }

        private IEnumerator FadeHighlight(Image highlight, float fromAlpha, float toAlpha)
        {
            float elapsed = 0f;
            Color c = highlightColour;

            while (elapsed < pulseDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(fromAlpha, toAlpha, Mathf.Clamp01(elapsed / pulseDuration));
                highlight.color = c;
                yield return null;
            }

            c.a = toAlpha;
            highlight.color = c;
        }

        private static string FormatScore(int score)
        {
            return score.ToString("#,##0");
        }
    }
}
