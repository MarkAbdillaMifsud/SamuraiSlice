using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "Game";

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
