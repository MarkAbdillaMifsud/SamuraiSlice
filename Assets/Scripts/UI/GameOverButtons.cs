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
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OnMainMenuPressed()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}