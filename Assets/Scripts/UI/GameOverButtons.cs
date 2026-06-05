using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class GameOverButtons : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Game";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        public void OnRetryPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OnMainMenuPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}