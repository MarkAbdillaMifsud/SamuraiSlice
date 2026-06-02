using System.Text;
using UnityEngine;

namespace SamuraiSlice
{
    public class InitialsEntryController : MonoBehaviour
    {
        [SerializeField] private CharacterSlot[] slots;
        [SerializeField] private GameObject initialsPanel;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private LeaderboardManager leaderboardManager;
        [SerializeField] private GameOverBinder gameOverBinder;

        private void Start()
        {
            
        }

        public void OnConfirmPressed()
        {
            var sb = new StringBuilder(slots.Length);
            foreach (CharacterSlot slot in slots)
            {
                sb.Append(slot.CurrentChar);
            }

            LeaderboardManager.SetPlayerName(sb.ToString());
            if(leaderboardManager != null)
            {
                leaderboardManager.SubmitLastRun();
            }

            SetPhase(initialsVisible: false);
            gameOverBinder?.ShowLeaderboard();
        }

        private void PrePopulateSlots()
        {
            string saved = LeaderboardManager.GetPlayerName(); 

            for (int i = 0; i < slots.Length; i++)
            {
                char c = (i < saved.Length) ? saved[i] : 'A';
                slots[i].SetChar(c);
            }
        }

        private void SetPhase(bool initialsVisible)
        {
            if (initialsPanel != null) initialsPanel.SetActive(initialsVisible);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(!initialsVisible);
        }
    }
}
