using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public enum GameState { Menu, Tutorial, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        [SerializeField] private Spawner spawner;
        [SerializeField] private MissCounter missCounter;
        [SerializeField] private TutorialOverlay tutorialOverlay;
        [SerializeField] private IngredientPool ingredientPool;
        [SerializeField] private int missThreshold = 3;
        [SerializeField] private string gameOverSceneName = "GameOver";
        [SerializeField] private float gameOverDelay = 1.5f;

        private bool _isEndingRun;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if(missCounter != null)
            {
                missCounter.OnMissCountChanged += HandleMissCountChanged;
            }
            Bomb.DetonationStarted += HandleBombDetonation;
            Bomb.Sliced += HandleBombSliced;
        }

        private void Start()
        {
            if(PlayerPrefs.GetInt("tutorial_seen_v1", 0) == 0 && tutorialOverlay != null)
            {
                SetState(GameState.Tutorial);
                tutorialOverlay.OnDismissed += HandleTutorialDismissed;
                tutorialOverlay.Show();
            } else
            {
                BeginPlaying();
            }
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }
            if(missCounter != null)
            {
                missCounter.OnMissCountChanged -= HandleMissCountChanged;
            }
            Bomb.DetonationStarted -= HandleBombDetonation;
            Bomb.Sliced -= HandleBombSliced;
            Instance = null;
        }

        private void Update()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //TODO: Remove after Main Menu implementation
            }
        }

        private void SetState(GameState newState)
        {
            if(CurrentState == newState)
            {
                return;
            }
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        public void Pause()
        {
            if(CurrentState != GameState.Playing)
            {
                return;
            }
            Time.timeScale = 0f;
            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (CurrentState != GameState.Paused)
            {
                return;
            }
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }

        private void HandleMissCountChanged(int misses)
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }
            if(misses >= missThreshold)
            {
                EndRun();
            }
        }

        private void HandleBombDetonation()
        {
            if(CurrentState != GameState.Playing)
            {
                return;
            }
            SetState(GameState.GameOver);
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
        }

        private void HandleBombSliced(Bomb bomb)
        {
            EndRun();
        }

        private void HandleTutorialDismissed()
        {
            tutorialOverlay.OnDismissed -= HandleTutorialDismissed;
            BeginPlaying();
        }

        private void BeginPlaying()
        {
            SetState(GameState.Playing);
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }

        private void EndRun()
        {
            if(_isEndingRun)
            {
                return;
            }
            _isEndingRun = true;

            SetState(GameState.GameOver);

            if(spawner != null)
            {
                spawner.StopSpawning();
            }

            if (ingredientPool != null)
            {
                ingredientPool.ReleaseAllActive();
            }

            StartCoroutine(LoadGameOverScene());
        }

        private IEnumerator LoadGameOverScene()
        {
            yield return new WaitForSecondsRealtime(gameOverDelay);
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
