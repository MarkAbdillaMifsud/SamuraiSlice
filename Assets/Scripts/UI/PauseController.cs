using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SamuraiSlice
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private SliceDetector sliceDetector;

        private void Start()
        {
            SetPanelVisible(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            } else
            {
                Debug.LogError("[PauseController] GameManager.Instance is null in Start. Ensure GameManager is in the scene and executes before this component.", this);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        public void Pause()
        {
            GameManager.Instance?.Pause();
        }

        public void Resume()
        {
            GameManager.Instance?.Resume();
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void HandleStateChanged(GameManager.GameState newState)
        {
            bool isPaused = newState == GameManager.GameState.Paused;
            bool isPlaying = newState == GameManager.GameState.Playing;

            SetPanelVisible(isPaused);

            if(sliceDetector != null)
            {
                sliceDetector.enabled = isPlaying;
            }

            if(pauseButton != null)
            {
                pauseButton.gameObject.SetActive(isPlaying);
            }
        }

        private void SetPanelVisible(bool visible)
        {
            if(pausePanel != null)
            {
                pausePanel.SetActive(visible);
            }
        }
    }
}
