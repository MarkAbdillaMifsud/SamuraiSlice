using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SamuraiSlice
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public enum GameState { Menu, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        [SerializeField] private Spawner spawner;
        [SerializeField] private MissCounter missCounter;
        [SerializeField] private int missThreshold = 3;


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
            Bomb.Sliced += HandleBombSliced;
        }

        private void Start()
        {
            SetState(GameState.Playing);
            if(spawner != null)
            {
                spawner.StartSpawning();
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
            Bomb.Sliced -= HandleBombSliced;
            Instance = null;
        }

        private void Update()
        {
            if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
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

        private void HandleBombSliced(Bomb bomb)
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }
            EndRun();
        }

        private void EndRun()
        {
            Debug.Log("GAME OVER!");
            SetState(GameState.GameOver);
            if(spawner != null)
            {
                spawner.StopSpawning();
            }

            foreach (var ingredient in FindObjectsByType<Ingredient>(FindObjectsInactive.Exclude))
            {
                ingredient.ForceRelease();
            }
            foreach (var bomb in FindObjectsByType<Bomb>(FindObjectsInactive.Exclude))
            {
                bomb.ForceRelease();
            }
        }
    }
}
