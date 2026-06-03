using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private GameObject scoresPanel;
        [SerializeField] private GameOverBinder gameOverBinder;

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnScoresClicked()
        {
            if (scoresPanel == null)
            {
                Debug.LogWarning("[MainMenuController] scoresPanel not assigned.", this);
                return;
            }

            scoresPanel.SetActive(true);
            gameOverBinder?.ShowLeaderboard();
        }

        public void OnScoresPanelClosed()
        {
            if (scoresPanel != null)
            {
                scoresPanel.SetActive(false);
            }
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
