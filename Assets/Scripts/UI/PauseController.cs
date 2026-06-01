using UnityEngine;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        public void Pause()
        {
            GameManager.Instance.Pause();
            pausePanel.SetActive(true);
        }

        public void Resume()
        {
            GameManager.Instance.Resume();
            pausePanel.SetActive(false);
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
