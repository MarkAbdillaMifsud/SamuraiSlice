using System.Collections;
using TMPro;
using UnityEngine;

namespace SamuraiSlice
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MissCounter missCounter;
        [SerializeField] private ComboTracker comboTracker;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text missText;
        [SerializeField] TMP_Text comboText;
        [SerializeField] private float comboHideDelay = 0.45f;
        [SerializeField] private float comboFadeDuration = 0.2f;

        private Coroutine _comboHideRoutine;

        const int MaxMisses = 3;

        private void OnEnable()
        {
            if(scoreManager != null)
            {
                scoreManager.OnScoreChanged += HandleScoreChanged;
                HandleScoreChanged(scoreManager.CurrentScore);
            }
            if (missCounter != null)
            {
                missCounter.OnMissCountChanged += HandleMissCountChanged;
                HandleMissCountChanged(missCounter.CurrentMisses);
            }
            if (comboTracker != null)
            {
                comboTracker.OnMultiplierChanged += HandleComboChanged;
                HandleComboChanged(comboTracker.CurrentMultiplier);
            }
        }

        private void OnDisable()
        {
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= HandleScoreChanged;
            }
            if (missCounter != null)
            {
                missCounter.OnMissCountChanged -= HandleMissCountChanged;
            }
            if (comboTracker != null)
            {
                comboTracker.OnMultiplierChanged -= HandleComboChanged;
            }
            if (_comboHideRoutine != null)
            {
                StopCoroutine(_comboHideRoutine);
                _comboHideRoutine = null;
            }
        }

        private void HandleScoreChanged(int newScore)
        {
            if(scoreText != null)
            {
                scoreText.text = newScore.ToString();
            }
        }

        private void HandleMissCountChanged(int newMisses)
        {
            if(missText != null)
            {
                missText.text = $"{newMisses}/{MaxMisses}";
            }
        }

        private void HandleComboChanged(int newMultiplier)
        {
            if (comboText == null)
            {
                return;
            }

            if (newMultiplier <= 1)
            {
                BeginHideCombo();
                return;
            }

            ShowCombo(newMultiplier);
        }

        private void ShowCombo(int multiplier)
        {
            if(_comboHideRoutine != null)
            {
                StopCoroutine(_comboHideRoutine);
                _comboHideRoutine = null;
            }

            comboText.gameObject.SetActive(true);
            comboText.alpha = 1f;
            comboText.text = $"x{multiplier}";
        }

        private void BeginHideCombo()
        {
            if(_comboHideRoutine != null)
            {
                StopCoroutine(_comboHideRoutine);
            }

            _comboHideRoutine = StartCoroutine(HideComboAfterDelay());
        }

        private IEnumerator HideComboAfterDelay()
        {
            yield return new WaitForSeconds(comboHideDelay);

            float elapsed = 0f;
            float startAlpha = comboText.alpha;

            while (elapsed < comboFadeDuration)
            {
                elapsed += Time.deltaTime;
                comboText.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / comboFadeDuration);
                yield return null;
            }

            comboText.alpha = 0f;
            comboText.text = string.Empty;
            comboText.gameObject.SetActive(false);
            _comboHideRoutine = null;
        }
    }
}
