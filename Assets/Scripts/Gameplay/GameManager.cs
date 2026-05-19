using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public enum GameState { Menu, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        [SerializeField] private Spawner spawner;
        [SerializeField] private int _missThreshold = 3;


        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            MissCounter.OnMissCountChanged += HandleMissCountChanged;
            Bomb.OnBombSliced += HandleBombSliced;

            SetState(GameState.Playing);
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
            if(CurrentState != GameState.Playing)
            {
                return;
            }
            if(misses >= _missThreshold)
            {
                EndRun();
            }
        }

        private void HandleBombSliced()
        {
            if(CurrentState != GameState.Playing)
            {
                return;
            }
            EndRun();
        }

        private void EndRun()
        {
            SetState(GameState.GameOver);
            if(spawner != null)
            {
                spawner.StopSpawning();
            }

            var ingredients = FindObjectsByType<Ingredient>();
            foreach (var ingredient in ingredients)
            {
                Destroy(ingredient.gameObject);

                Debug.Log("GAME OVER!");
            }
        }
    }
}
