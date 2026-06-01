using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class HighScoreManager : MonoBehaviour
    {
        private const string PrefsKey = "SamuraiSlice_HighScore";
        private const string LastScoreKey = "SamuraiSlice_LastScore";

        [SerializeField] private ScoreManager scoreManager;

        public int HighScore { get; private set; }

        public event Action<int> OnHighScoreChanged;
        public event Action<int> OnNewHighScore;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(PrefsKey, 0);

            if(scoreManager == null)
            {
                Debug.LogError("[HighScoreManager] ScoreManager reference not assigned.", this);
            }
        }

        private void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[HighScoreManager] GameManager.Instance is null in Start.", this);
                return;
            }

            GameManager.Instance.OnStateChanged += HandleStateChanged;

            OnHighScoreChanged?.Invoke(HighScore);
        }

        private void OnDestroy()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameManager.GameState newState)
        {
            if(newState != GameManager.GameState.GameOver)
            {
                return;
            }

            if(scoreManager == null)
            {
                return;
            }

            int finalScore = scoreManager.CurrentScore;

            PlayerPrefs.SetInt(LastScoreKey, finalScore);

            if (finalScore > HighScore)
            {
                HighScore = finalScore;
                PlayerPrefs.SetInt(PrefsKey, HighScore);
                OnNewHighScore?.Invoke(HighScore);
                OnHighScoreChanged?.Invoke(HighScore);
            }

            PlayerPrefs.Save();
        }

        public void ResetHighScore()
        {
            HighScore = 0;
            PlayerPrefs.SetInt(PrefsKey, 0);
            PlayerPrefs.Save();
            OnHighScoreChanged?.Invoke(HighScore);
        }
    }
}
