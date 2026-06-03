using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SamuraiSlice
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject[] pages;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private TMP_Text nextButtonLabel;
        [SerializeField] private string advanceLabel = "Next";
        [SerializeField] private string finalLabel = "Play!";

        public event Action OnDismissed;

        private int _currentPage;

        private void OnEnable()
        {
            if(nextButton  != null)
            {
                nextButton.onClick.AddListener(Advance);
            }

            if(skipButton != null)
            {
                skipButton.onClick.AddListener(Skip);
            }
        }

        private void OnDisable()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(Advance);
            }

            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(Skip);
            }
        }

        public void Show()
        {
            _currentPage = 0;
            ShowPage(_currentPage);
            gameObject.SetActive(true);
        }

        private void Advance()
        {
            _currentPage++;

            if(_currentPage < pages.Length)
            {
                ShowPage(_currentPage);
            } else
            {
                Dismiss();
            }
        }

        private void Skip()
        {
            Dismiss();
        }

        private void ShowPage(int index)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].SetActive(i == index);
                }
            }

            if(nextButtonLabel != null)
            {
                nextButtonLabel.text = (index == pages.Length - 1) ? finalLabel : advanceLabel;
            }
        }

        private void Dismiss()
        {
            PlayerPrefs.SetInt("tutorial_seen_v1", 1);
            PlayerPrefs.Save();
            gameObject.SetActive(false);
            OnDismissed?.Invoke();
        }
    }

}