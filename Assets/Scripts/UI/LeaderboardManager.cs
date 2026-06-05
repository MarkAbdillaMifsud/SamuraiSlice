using System;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSlice
{
    public class LeaderboardManager : MonoBehaviour
    {
        private const string LeaderboardKey = "SamuraiSlice_Leaderboard";
        private const string HighScoreKey = "SamuraiSlice_HighScore";
        private const string LastScoreKey = "SamuraiSlice_LastScore";
        private const string PlayerNameKey = "SamuraiSlice_PlayerName";
        private const int MaxEntries = 5;

        [Serializable]
        public class Entry
        {
            public string name;
            public int score;
        }

        [Serializable]
        public class LeaderboardData
        {
            public List<Entry> entries = new List<Entry>();
        }

        private LeaderboardData _data;

        public IReadOnlyList<Entry> Entries => _data.entries;

        public static int GetLastScore()
        {
            return PlayerPrefs.GetInt(LastScoreKey, 0);
        }

        private void Awake()
        {
            _data = Load();
            SortandTrim();
        }

        public int SubmitLastRun()
        {
            int score = GetLastScore();

            if(!Qualifies(score))
            {
                return -1;
            }

            string playerName = GetPlayerName();
            var newEntry = new Entry { name = playerName, score = score };

            _data.entries.Add(newEntry);
            SortandTrim();

            int rank = -1;
            for(int i = 0; i < Math.Min(_data.entries.Count, MaxEntries); i++)
            {
                if(ReferenceEquals(_data.entries[i], newEntry))
                {
                    rank = i + 1;
                    break;
                }
            }

            Save();

            if(_data.entries.Count > 0)
            {
                PlayerPrefs.SetInt(HighScoreKey, _data.entries[0].score);
            }

            PlayerPrefs.SetInt(LastScoreKey, 0);
            PlayerPrefs.Save();

            return rank;
        }

        public bool Qualifies(int score)
        {
            if(score <= 0)
            {
                return false;
            }

            SortandTrim();

            if(_data.entries.Count < MaxEntries)
            {
                return true;
            }

            return score > _data.entries[MaxEntries - 1].score;
        }

        public static string GetPlayerName()
        {
            return PlayerPrefs.GetString(PlayerNameKey, "YOU");
        }

        public static void SetPlayerName(string name)
        {
            string trimmed = name.Trim().ToUpper();
            if (trimmed.Length > 3) trimmed = trimmed.Substring(0, 3);
            PlayerPrefs.SetString(PlayerNameKey, trimmed);
            PlayerPrefs.Save();
        }

        public void ResetLeaderboard()
        {
            _data = new LeaderboardData();
            PlayerPrefs.DeleteKey(LeaderboardKey);
            PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.Save();
        }

        private void SortandTrim()
        {
            _data.entries.Sort((a, b) => b.score.CompareTo(a.score));

            if(_data.entries.Count > MaxEntries)
            {
                _data.entries.RemoveRange(MaxEntries, _data.entries.Count - MaxEntries);
            }
        }

        private void Save()
        {
            string json = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(LeaderboardKey, json);
        }

        private static LeaderboardData Load()
        {
            string json = PlayerPrefs.GetString(LeaderboardKey, string.Empty);
            if(string.IsNullOrEmpty(json))
            {
                return new LeaderboardData();
            }

            try
            {
                return JsonUtility.FromJson<LeaderboardData>(json);
            } catch
            {
                Debug.LogWarning("[LeaderboardManager] Failed to parse leaderboard JSON. Resetting.");
                return new LeaderboardData();
            }
        }
    }
}
